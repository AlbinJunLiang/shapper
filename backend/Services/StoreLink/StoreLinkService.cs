using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.StoreLinks;
using Shapper.Models;
using Shapper.Repositories.StoreLinks;

namespace Shapper.Services.StoreLinks
{
    public class StoreLinkService : IStoreLinkService
    {
        private readonly IStoreLinkRepository _storeLinkRepository;
        private readonly IMapper _mapper;

        public StoreLinkService(IStoreLinkRepository storeLinkRepository, IMapper mapper)
        {
            _storeLinkRepository = storeLinkRepository;
            _mapper = mapper;
        }

        public async Task<StoreLinkResponseDto> CreateAsync(StoreLinkDto dto)
        {
            // Validación 1: StoreInformation existe
            if (!await _storeLinkRepository.StoreExistsAsync(dto.StoreId))
                throw new InvalidOperationException(
                    $"StoreInformation with ID {dto.StoreId} does not exist."
                );

            // Validación 2: Nombre único dentro de la misma tienda
            var existing = await _storeLinkRepository.GetByNameAndStoreAsync(dto.Name, dto.StoreId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"A link with name '{dto.Name}' already exists for this store."
                );

            // Validación 3: Límite de links por tienda (opcional - 20 links máximo)
            var linkCount = await _storeLinkRepository.CountLinksByStoreAsync(dto.StoreId);
            if (linkCount >= 20)
                throw new InvalidOperationException(
                    "Maximum number of links (20) reached for this store."
                );

            var storeLink = _mapper.Map<StoreLink>(dto);
            storeLink.CreatedAt = DateTime.UtcNow;

            await _storeLinkRepository.AddAsync(storeLink);

            var created = await _storeLinkRepository.GetByIdAsync(storeLink.Id);
            return _mapper.Map<StoreLinkResponseDto>(created);
        }

        public async Task<StoreLinkResponseDto?> GetByIdAsync(int id)
        {
            var storeLink = await _storeLinkRepository.GetByIdAsync(id);
            return storeLink == null ? null : _mapper.Map<StoreLinkResponseDto>(storeLink);
        }

        public async Task<List<StoreLinkResponseDto>> GetByStoreIdAsync(int storeId)
        {
            var storeLinks = await _storeLinkRepository.GetByStoreIdAsync(storeId);
            return _mapper.Map<List<StoreLinkResponseDto>>(storeLinks);
        }

        public async Task<PagedResponseDto<StoreLinkResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeId = null
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (storeLinks, totalCount) = await _storeLinkRepository.GetPaginatedAsync(
                page,
                pageSize,
                storeId
            );
            var mapped = _mapper.Map<List<StoreLinkResponseDto>>(storeLinks);

            return new PagedResponseDto<StoreLinkResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<StoreLinkResponseDto> UpdateAsync(int id, StoreLinkUpdateDto dto)
        {
            var existing = await _storeLinkRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"StoreLink with ID {id} not found.");

            // Validación: Si se actualiza el nombre, verificar que sea único en la tienda
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != existing.Name)
            {
                var nameExists = await _storeLinkRepository.GetByNameAndStoreAsync(
                    dto.Name,
                    existing.StoreId,
                    id
                );
                if (nameExists != null)
                    throw new InvalidOperationException(
                        $"A link with name '{dto.Name}' already exists for this store."
                    );
                existing.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Url))
                existing.Url = dto.Url;

            if (!string.IsNullOrEmpty(dto.Type))
                existing.Type = dto.Type;

            if (!string.IsNullOrEmpty(dto.Status))
                existing.Status = dto.Status;

            existing.UpdatedAt = DateTime.UtcNow;

            await _storeLinkRepository.UpdateAsync(existing);

            var updated = await _storeLinkRepository.GetByIdAsync(id);
            return _mapper.Map<StoreLinkResponseDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var storeLink = await _storeLinkRepository.GetByIdAsync(id);
            if (storeLink == null)
                throw new InvalidOperationException($"StoreLink with ID {id} not found.");

            await _storeLinkRepository.DeleteAsync(storeLink);
        }

        public async Task<StoreLinkResponseDto> ToggleStatusAsync(int id)
        {
            var storeLink = await _storeLinkRepository.GetByIdAsync(id);
            if (storeLink == null)
                throw new InvalidOperationException($"StoreLink with ID {id} not found.");

            storeLink.Status = storeLink.Status == "ACTIVE" ? "INACTIVE" : "ACTIVE";
            storeLink.UpdatedAt = DateTime.UtcNow;

            await _storeLinkRepository.UpdateAsync(storeLink);

            return _mapper.Map<StoreLinkResponseDto>(storeLink);
        }

        public async Task<StoreLinkResponseDto> UpsertAsync(int? id, StoreLinkUpdateDto dto)
        {
            StoreLink? entity;
            bool isNew = false;

            // 1. Identificación: El ID viene de la URL, no del DTO
            if (id.HasValue && id > 0)
            {
                // CASO UPDATE: Recuperamos la entidad original de la BD
                entity = await _storeLinkRepository.GetByIdAsync(id.Value);

                if (entity == null)
                    throw new InvalidOperationException($"StoreLink with ID {id} not found.");

                // Nunca asignamos entity.Id = algo.
                // EF ya sabe que entity.Id es el valor actual y lo marcará como inmutable.
            }
            else
            {
                // CASO CREATE
                isNew = true;

                // Validamos que la tienda exista para evitar el error de Foreign Key
                if (!await _storeLinkRepository.StoreExistsAsync(dto.StoreId))
                    throw new InvalidOperationException(
                        $"Store with ID {dto.StoreId} does not exist."
                    );

                entity = new StoreLink { StoreId = dto.StoreId, CreatedAt = DateTime.UtcNow };

                // Al ser un objeto nuevo, el ID es 0 y SQL Server lo generará (Identity)
            }

            // 2. Mapeo de datos (Solo campos de negocio, NUNCA el ID)
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Url))
                entity.Url = dto.Url;
            if (!string.IsNullOrEmpty(dto.Type))
                entity.Type = dto.Type;
            if (!string.IsNullOrEmpty(dto.Status))
                entity.Status = dto.Status;

            entity.UpdatedAt = DateTime.UtcNow;

            // 3. Persistencia
            if (isNew)
                await _storeLinkRepository.AddAsync(entity);
            else
                await _storeLinkRepository.UpdateAsync(entity);

            return _mapper.Map<StoreLinkResponseDto>(entity);
        }
    }
}
