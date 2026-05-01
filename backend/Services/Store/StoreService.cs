using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Store;
using Shapper.Repositories.Stores;

namespace Shapper.Services.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public StoreService(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<StoreResponseDto?> CreateAsync(StoreDto dto)
        {
            // Validación 1: Nombre único
            if (!await _storeRepository.IsNameUniqueAsync(dto.Name))
                throw new InvalidOperationException(
                    $"A store with the name '{dto.Name}' already exists."
                );

            // Validación 2: Email único
            if (!await _storeRepository.IsEmailUniqueAsync(dto.Email))
                throw new InvalidOperationException(
                    $"A store with the email '{dto.Email}' already exists."
                );

            var storeInfo = _mapper.Map<Models.Store>(dto);
            storeInfo.CreatedAt = DateTime.UtcNow;

            await _storeRepository.AddAsync(storeInfo);

            var created = await _storeRepository.GetByIdAsync(storeInfo.Id);
            return _mapper.Map<StoreResponseDto>(created);
        }

        public async Task<StoreResponseDto?> GetByIdAsync(int id)
        {
            var storeInfo = await _storeRepository.GetByIdAsync(id);
            return storeInfo == null ? null : _mapper.Map<StoreResponseDto>(storeInfo);
        }

        public async Task<StoreResponseDto?> GetByStoreCodeAsync(string storeCode)
        {
            var storeInfo = await _storeRepository.GetByStoreCodeAsync(storeCode);
            return storeInfo == null ? null : _mapper.Map<StoreResponseDto>(storeInfo);
        }

        public async Task<PagedResponseDto<StoreResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (stores, totalCount) = await _storeRepository.GetPaginatedAsync(page, pageSize);
            var mapped = _mapper.Map<List<StoreResponseDto>>(stores);

            return new PagedResponseDto<StoreResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<StoreResponseDto> UpdateAsync(int id, StoreDto dto)
        {
            var existing = await _storeRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"store with ID {id} not found.");

            // Validación 1: Nombre único (excluyendo el actual)
            if (
                existing.Name != dto.Name
                && !await _storeRepository.IsNameUniqueAsync(dto.Name, id)
            )
                throw new InvalidOperationException(
                    $"A store with the name '{dto.Name}' already exists."
                );

            // Validación 2: Email único (excluyendo el actual)
            if (
                existing.Email != dto.Email
                && !await _storeRepository.IsEmailUniqueAsync(dto.Email, id)
            )
                throw new InvalidOperationException(
                    $"A store with the email '{dto.Email}' already exists."
                );

            _mapper.Map(dto, existing);
            existing.Id = id;

            await _storeRepository.UpdateAsync(existing);

            var updated = await _storeRepository.GetByIdAsync(id);
            return _mapper.Map<StoreResponseDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var storeInfo = await _storeRepository.GetByIdAsync(id);
            if (storeInfo == null)
                throw new InvalidOperationException($"store with ID {id} not found.");

            // Validación: Verificar si tiene StoreLinks asociados
            if (storeInfo.StoreLinks != null && storeInfo.StoreLinks.Any())
                throw new InvalidOperationException(
                    "Cannot delete store with associated links. Remove all links first."
                );

            await _storeRepository.DeleteAsync(storeInfo);
        }
    }
}
