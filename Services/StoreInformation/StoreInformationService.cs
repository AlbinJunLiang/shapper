using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.StoreInformations;
using Shapper.Models;
using Shapper.Repositories.StoreInformations;

namespace Shapper.Services.StoreInformations
{
    public class StoreInformationService : IStoreInformationService
    {
        private readonly IStoreInformationRepository _storeInformationRepository;
        private readonly IMapper _mapper;

        public StoreInformationService(
            IStoreInformationRepository storeInformationRepository,
            IMapper mapper
        )
        {
            _storeInformationRepository = storeInformationRepository;
            _mapper = mapper;
        }

        public async Task<StoreInformationResponseDto?> CreateAsync(StoreInformationDto dto)
        {
            // Validación 1: Nombre único
            if (!await _storeInformationRepository.IsNameUniqueAsync(dto.Name))
                throw new InvalidOperationException(
                    $"A store with the name '{dto.Name}' already exists."
                );

            // Validación 2: Email único
            if (!await _storeInformationRepository.IsEmailUniqueAsync(dto.Email))
                throw new InvalidOperationException(
                    $"A store with the email '{dto.Email}' already exists."
                );

            var storeInfo = _mapper.Map<StoreInformation>(dto);
            storeInfo.CreatedAt = DateTime.UtcNow;

            await _storeInformationRepository.AddAsync(storeInfo);

            var created = await _storeInformationRepository.GetByIdAsync(storeInfo.Id);
            return _mapper.Map<StoreInformationResponseDto>(created);
        }

        public async Task<StoreInformationResponseDto?> GetByIdAsync(int id)
        {
            var storeInfo = await _storeInformationRepository.GetByIdAsync(id);
            return storeInfo == null ? null : _mapper.Map<StoreInformationResponseDto>(storeInfo);
        }

        public async Task<PagedResponseDto<StoreInformationResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (storeInformations, totalCount) =
                await _storeInformationRepository.GetPaginatedAsync(page, pageSize);
            var mapped = _mapper.Map<List<StoreInformationResponseDto>>(storeInformations);

            return new PagedResponseDto<StoreInformationResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<StoreInformationResponseDto> UpdateAsync(int id, StoreInformationDto dto)
        {
            var existing = await _storeInformationRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"StoreInformation with ID {id} not found.");

            // Validación 1: Nombre único (excluyendo el actual)
            if (
                existing.Name != dto.Name
                && !await _storeInformationRepository.IsNameUniqueAsync(dto.Name, id)
            )
                throw new InvalidOperationException(
                    $"A store with the name '{dto.Name}' already exists."
                );

            // Validación 2: Email único (excluyendo el actual)
            if (
                existing.Email != dto.Email
                && !await _storeInformationRepository.IsEmailUniqueAsync(dto.Email, id)
            )
                throw new InvalidOperationException(
                    $"A store with the email '{dto.Email}' already exists."
                );

            _mapper.Map(dto, existing);
            existing.Id = id;

            await _storeInformationRepository.UpdateAsync(existing);

            var updated = await _storeInformationRepository.GetByIdAsync(id);
            return _mapper.Map<StoreInformationResponseDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var storeInfo = await _storeInformationRepository.GetByIdAsync(id);
            if (storeInfo == null)
                throw new InvalidOperationException($"StoreInformation with ID {id} not found.");

            // Validación: Verificar si tiene StoreLinks asociados
            if (storeInfo.StoreLinks != null && storeInfo.StoreLinks.Any())
                throw new InvalidOperationException(
                    "Cannot delete store with associated links. Remove all links first."
                );

            await _storeInformationRepository.DeleteAsync(storeInfo);
        }
    }
}
