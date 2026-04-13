using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;

using Shapper.Models;
using Shapper.Repositories.OrderDetails;

namespace Shapper.Services.OrderDetails
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        public async Task<OrderDetailResponseDto> CreateAsync(OrderDetailDto dto)
        {
            var category = _mapper.Map<OrderDetail>(dto);

            await _orderDetailRepository.AddAsync(category);

            return _mapper.Map<OrderDetailResponseDto>(category);
        }

        public async Task<OrderDetailDto?> GetByIdAsync(int id)
        {
            var category = await _orderDetailRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<OrderDetailDto>(category);
        }

        public async Task<PagedResponseDto<OrderDetailResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (OrderDetails, totalCount) = await _orderDetailRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<OrderDetailResponseDto>>(OrderDetails);

            return new PagedResponseDto<OrderDetailResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<OrderDetailResponseDto> UpdateAsync(int id, OrderDetailDto dto)
        {
            var existingOrder = await _orderDetailRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("OrderDetail not found.");
            _mapper.Map(dto, existingOrder);

            await _orderDetailRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<OrderDetailResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _orderDetailRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("OrderDetail not found.");

            await _orderDetailRepository.DeleteAsync(category);
        }
    }
}
