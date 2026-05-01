using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Locations;
using Shapper.Dtos.Locations;


namespace Shapper.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<LocationResponseDto?> CreateAsync(LocationDto dto)
        {
            var category = _mapper.Map<Location>(dto);

            await _locationRepository.AddAsync(category);

            return _mapper.Map<LocationResponseDto>(category);
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            var category = await _locationRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<LocationDto>(category);
        }

        public async Task<PagedResponseDto<LocationResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (Locations, totalCount) = await _locationRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<LocationResponseDto>>(Locations);

            return new PagedResponseDto<LocationResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<LocationResponseDto> UpdateAsync(int id, LocationDto dto)
        {
            var existingOrder = await _locationRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("Location not found.");
            _mapper.Map(dto, existingOrder);

            await _locationRepository.UpdateAsync(existingOrder);
            return _mapper.Map<LocationResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _locationRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("Location not found.");

            await _locationRepository.DeleteAsync(category);
        }
    }
}
