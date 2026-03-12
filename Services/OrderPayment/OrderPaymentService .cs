using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.OrderPayments;

namespace Shapper.Services.OrderPayments
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IMapper _mapper;

        public OrderPaymentService(IOrderPaymentRepository orderPaymentRepository, IMapper mapper)
        {
            _orderPaymentRepository = orderPaymentRepository;
            _mapper = mapper;
        }

        public async Task<OrderPaymentResponseDto> CreateAsync(OrderPaymentDto dto)
        {
            var category = _mapper.Map<OrderPayment>(dto);

            await _orderPaymentRepository.AddAsync(category);

            return _mapper.Map<OrderPaymentResponseDto>(category);
        }

        public async Task<OrderPaymentDto?> GetByIdAsync(int id)
        {
            var category = await _orderPaymentRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<OrderPaymentDto>(category);
        }

        public async Task<PagedResponseDto<OrderPaymentResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (OrderPayments, totalCount) = await _orderPaymentRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<OrderPaymentResponseDto>>(OrderPayments);

            return new PagedResponseDto<OrderPaymentResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<OrderPaymentResponseDto> UpdateAsync(int id, OrderPaymentDto dto)
        {
            var existingOrder = await _orderPaymentRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("OrderPayment not found.");
            _mapper.Map(dto, existingOrder);

            await _orderPaymentRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<OrderPaymentResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _orderPaymentRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("OrderPayment not found.");

            await _orderPaymentRepository.DeleteAsync(category);
        }
    }
}
