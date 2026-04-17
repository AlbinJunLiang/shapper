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
            if (!await _storeLinkRepository.StoreInformationExistsAsync(dto.StoreInformationId))
                throw new InvalidOperationException(
                    $"StoreInformation with ID {dto.StoreInformationId} does not exist."
                );

            // Validación 2: Nombre único dentro de la misma tienda
            var existing = await _storeLinkRepository.GetByNameAndStoreAsync(
                dto.Name,
                dto.StoreInformationId
            );
            if (existing != null)
                throw new InvalidOperationException(
                    $"A link with name '{dto.Name}' already exists for this store."
                );

            // Validación 3: Límite de links por tienda (opcional - 20 links máximo)
            var linkCount = await _storeLinkRepository.CountLinksByStoreAsync(
                dto.StoreInformationId
            );
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

        public async Task<List<StoreLinkResponseDto>> GetByStoreIdAsync(int storeInformationId)
        {
            var storeLinks = await _storeLinkRepository.GetByStoreIdAsync(storeInformationId);
            return _mapper.Map<List<StoreLinkResponseDto>>(storeLinks);
        }

        public async Task<PagedResponseDto<StoreLinkResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeInformationId = null
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (storeLinks, totalCount) = await _storeLinkRepository.GetPaginatedAsync(
                page,
                pageSize,
                storeInformationId
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
                    existing.StoreInformationId,
                    id
                );
                if (nameExists != null)
                    throw new InvalidOperationException(
                        $"A link with name '{dto.Name}' already exists for this store."
                    );
                existing.Name = dto.Name;
            }

            // Actualizar campos si vienen en el DTO
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
    }
}
