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

        public async Task<StoreInformationResponseDto> CreateAsync(StoreInformationDto dto)
        {
            var category = _mapper.Map<StoreInformation>(dto);

            await _storeInformationRepository.AddAsync(category);

            return _mapper.Map<StoreInformationResponseDto>(category);
        }

        public async Task<StoreInformationDto?> GetByIdAsync(int id)
        {
            var category = await _storeInformationRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<StoreInformationDto>(category);
        }

        public async Task<PagedResponseDto<StoreInformationResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (StoreInformations, totalCount) =
                await _storeInformationRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<StoreInformationResponseDto>>(StoreInformations);

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
            var existingOrder = await _storeInformationRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("StoreInformation not found.");
            _mapper.Map(dto, existingOrder);

            await _storeInformationRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<StoreInformationResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _storeInformationRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("StoreInformation not found.");

            await _storeInformationRepository.DeleteAsync(category);
        }
    }
}
