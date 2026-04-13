using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Models;
using Shapper.Repositories.OrderPayments;
using Shapper.Repositories.Orders;

namespace Shapper.Services.OrderPayments
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly IMapper _mapper;

        public OrderPaymentService(
            IOrderPaymentRepository orderPaymentRepository,
            IOrderRepository orderRepository,
            IMapper mapper
        )
        {
            _orderPaymentRepository = orderPaymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(ConfirmPaymentDto orderData)
        {
            var order = await _orderRepository.GetByIdAsync(orderData.Order.Id);
            if (order == null)
                return false;

            var existing = await _orderPaymentRepository.GetByTransactionReferenceAsync(
                orderData.PaidId
            );

            if (existing != null || order.Status == "PAID")
                return true;

            order.Status = "PAID";

            var payment = new OrderPayment
            {
                OrderId = order.Id,
                TransactionReference = orderData.PaidId,
                Subtotal = orderData.Order.Subtotal,
                TaxAmount = orderData.Order.TotalTax,
                TotalAmount = orderData.Order.Total,
                PaidAt = DateTime.UtcNow,
                PaymentMethod = orderData.PaymentMethod,
                Status = "COMPLETED",
            };

            try
            {
                await _orderRepository.UpdateAsync(order);
                await _orderPaymentRepository.AddAsync(payment);

                await _orderRepository.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<OrderPaymentDto?> GetByIdAsync(int id)
        {
            var category = await _orderPaymentRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<OrderPaymentDto>(category);
        }

        public async Task<OrderPaymentDto?> GetByTransactionReferenceAsync(
            string transactionReference
        )
        {
            var category = await _orderPaymentRepository.GetByTransactionReferenceAsync(
                transactionReference
            );
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
