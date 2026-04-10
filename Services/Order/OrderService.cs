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

        public async Task<CartResponseDto> ValidateAndCalculateAsync(List<CartItemDto> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();

            // Repository: solo obtiene los datos
            var products = await _orderRepository.GetProductsByIdsAsync(productIds);

            var response = new CartResponseDto();
            decimal globalSubtotal = 0;
            decimal globalTaxTotal = 0;
            decimal globalDiscountTotal = 0;
            bool allAvailable = true;

            foreach (var item in items)
            {
                var validation = new CartItemValidationDto
                {
                    ProductId = item.ProductId,
                    RequestedQuantity = item.Quantity,
                };

                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    validation.IsAvailable = false;
                    validation.ErrorMessage = $"Producto con ID {item.ProductId} no existe";
                    allAvailable = false;
                }
                else
                {
                    // Service: toda la lógica de negocio aquí
                    validation.ProductName = product.Name;
                    validation.UnitPrice = (decimal)product.Price;
                    validation.TaxAmount = (decimal)product.TaxAmount;
                    validation.Discount = (decimal)product.Discount;
                    validation.AvailableQuantity = product.Quantity;

                    if (product.Quantity < item.Quantity)
                    {
                        validation.IsAvailable = false;
                        validation.ErrorMessage =
                            $"Stock insuficiente. Disponible: {product.Quantity}";
                        allAvailable = false;
                    }
                    else
                    {
                        validation.IsAvailable = true;

                        // Cálculos en el Service
                        var priceWithDiscount =
                            validation.UnitPrice * (1 - validation.Discount / 100);
                        validation.Subtotal = priceWithDiscount * item.Quantity;
                        validation.TaxTotal = validation.Subtotal * (validation.TaxAmount / 100);
                        validation.DiscountTotal =
                            (validation.UnitPrice * validation.Discount / 100) * item.Quantity;
                        validation.ItemTotal = validation.Subtotal + validation.TaxTotal;

                        globalSubtotal += validation.Subtotal;
                        globalTaxTotal += validation.TaxTotal;
                        globalDiscountTotal += validation.DiscountTotal;
                    }
                }

                response.Items.Add(validation);
            }

            response.Subtotal = globalSubtotal;
            response.TaxTotal = globalTaxTotal;
            response.DiscountTotal = globalDiscountTotal;
            response.Total = globalSubtotal + globalTaxTotal;
            response.AllItemsAvailable = allAvailable;

            return response;
        }
    }
}
