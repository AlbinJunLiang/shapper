using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Orders;

namespace Shapper.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateAsync(OrderDto dto)
        {
            var category = _mapper.Map<Order>(dto);

            await _orderRepository.AddAsync(category);

            return _mapper.Map<OrderResponseDto>(category);
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var category = await _orderRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<OrderDto>(category);
        }

        public async Task<PagedResponseDto<OrderResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (Orders, totalCount) = await _orderRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<OrderResponseDto>>(Orders);

            return new PagedResponseDto<OrderResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<OrderResponseDto> UpdateAsync(int id, OrderDto dto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("Order not found.");
            _mapper.Map(dto, existingOrder);

            await _orderRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<OrderResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _orderRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("Order not found.");

            await _orderRepository.DeleteAsync(category);
        }
    }
}
