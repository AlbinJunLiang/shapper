using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.Orders;
using Shapper.Helpers;
using Shapper.Models;
using Shapper.Repositories.Orders;
using Shapper.Repositories.Users;

namespace Shapper.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;

            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
        {
            await ValidateCustomerAsync(dto.CustomerId);

            var items = dto.Items;
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _orderRepository.GetProductsByIdsAsync(productIds);

            double totalOrder = 0;
            double totalSubtotal = 0; // Precio base - Descuento
            double totalDiscount = 0;
            double totalTax = 0;

            var orderDetailsResponse = new List<OrderDetailResponseDto>();

            var order = new Order
            {
                CustomerId = (dto.CustomerId <= 0) ? null : dto.CustomerId,
                OrderReference = OrderHelper.GenerateReference("ORD"),
                Status = "PENDING",
                Total = 0,
                CreatedAt = DateTime.UtcNow,
                ExtraData = dto.ExtraData,
                OrderDetails = new List<OrderDetail>(),
            };

            // 3. Procesamiento de productos y lógica de precios
            foreach (var item in items)
            {
                var detailResponse = new OrderDetailResponseDto
                {
                    ProductId = item.ProductId,
                    RequestedQuantity = item.Quantity,
                    Description = item.Description ?? string.Empty,
                    ProductImageUrl = item.ProductImageUrl ?? string.Empty,
                };

                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    detailResponse.Status = "PRODUCT_NOT_FOUND";
                    orderDetailsResponse.Add(detailResponse);
                    continue;
                }

                // --- Cálculos Unitarios ---
                var basePrice = product.Price;
                var discountPerUnit = basePrice * (product.Discount / 100);
                var priceAfterDiscount = basePrice - discountPerUnit;
                var taxPerUnit = priceAfterDiscount * (product.TaxAmount / 100);
                var finalPrice = priceAfterDiscount + taxPerUnit;

                detailResponse.ProductName = product.Name;
                detailResponse.BasePrice = basePrice;
                detailResponse.Discount = product.Discount;
                detailResponse.Tax = product.TaxAmount;
                // Redondeamos el precio unitario final para el DTO
                detailResponse.FinalPrice = Math.Round(
                    finalPrice,
                    2,
                    MidpointRounding.AwayFromZero
                );

                // --- Gestión de Stock ---
                int quantityToProcess = Math.Min(product.Quantity, item.Quantity);
                string status = product.Quantity < item.Quantity ? "OUT_OF_STOCK" : "PROCESSED";

                detailResponse.Status = status;
                detailResponse.ActualQuantity = quantityToProcess;

                if (quantityToProcess > 0)
                {
                    // Cálculos de la línea actual (Subtotal, Impuesto y Total de línea)
                    var lineSubtotal = priceAfterDiscount * quantityToProcess;
                    var lineTax = taxPerUnit * quantityToProcess;
                    var lineFinal = finalPrice * quantityToProcess;

                    // Acumulación global para el encabezado de la orden
                    totalSubtotal += lineSubtotal;
                    totalDiscount += (discountPerUnit * quantityToProcess);
                    totalTax += lineTax;
                    totalOrder += lineFinal;

                    // Agregamos el detalle a la entidad de base de datos
                    order.OrderDetails.Add(
                        new OrderDetail
                        {
                            ProductId = item.ProductId,
                            Quantity = quantityToProcess,
                            Price = Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero),
                            Subtotal = Math.Round(lineFinal, 2, MidpointRounding.AwayFromZero),
                            Description = item.Description ?? string.Empty,
                            Status = status,
                        }
                    );

                    detailResponse.Subtotal = Math.Round(
                        lineFinal,
                        2,
                        MidpointRounding.AwayFromZero
                    );

                    // Actualización de stock en memoria para el SaveChanges posterior
                    product.Quantity -= quantityToProcess;
                }

                orderDetailsResponse.Add(detailResponse);
            }

            // 4. Verificación de seguridad: No permitir órdenes sin productos válidos
            if (!order.OrderDetails.Any())
            {
                throw new InvalidOperationException(
                    "No valid products with stock found to create the order."
                );
            }

            // 5. Asignación final con redondeo a 2 decimales (Vital para PayPal/Stripe)
            order.Total = Math.Round(totalOrder, 2, MidpointRounding.AwayFromZero);

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // 6. Retorno del DTO con todos los totales calculados
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderReference = order.OrderReference,
                CustomerId = order.CustomerId,
                // Nuevos campos del DTO:
                Subtotal = Math.Round(totalSubtotal, 2, MidpointRounding.AwayFromZero),
                TotalDiscount = Math.Round(totalDiscount, 2, MidpointRounding.AwayFromZero),
                TotalTax = Math.Round(totalTax, 2, MidpointRounding.AwayFromZero),
                Total = order.Total,
                ExtraData = order.ExtraData,

                Status = order.Status,
                CompanyName = dto.CompanyName ?? string.Empty,
                CreatedAt = order.CreatedAt,
                Details = orderDetailsResponse,
            };
        }

        public async Task<OrderResponseDto?> GetByReferenceAsync(string reference)
        {
            var order = await _orderRepository.GetByReferenceAsync(reference);

            if (order == null)
                return null;

            double totalSubtotal = 0;
            double totalTax = 0;
            double totalDiscount = 0;

            var detailsDto = new List<OrderDetailResponseDto>();

            // 2. Recorremos los detalles guardados para reconstruir los totales
            foreach (var detail in order.OrderDetails)
            {
                var product = detail.Product;

                // Re-calculamos los montos unitarios basados en la foto del momento (el producto)
                var basePrice = product.Price;
                var discountPerUnit = basePrice * (product.Discount / 100);
                var priceAfterDiscount = basePrice - discountPerUnit;
                var taxPerUnit = priceAfterDiscount * (product.TaxAmount / 100);
                var finalPrice = priceAfterDiscount + taxPerUnit;

                // Acumulamos los totales globales
                totalSubtotal += (priceAfterDiscount * detail.Quantity);
                totalDiscount += (discountPerUnit * detail.Quantity);
                totalTax += (taxPerUnit * detail.Quantity);

                // Mapeamos el detalle al DTO
                detailsDto.Add(
                    new OrderDetailResponseDto
                    {
                        ProductId = detail.ProductId,
                        ProductName = product.Name,
                        RequestedQuantity = detail.Quantity, // Aquí asumimos que la guardada es la final
                        ActualQuantity = detail.Quantity,
                        BasePrice = basePrice,
                        Discount = product.Discount,
                        Tax = product.TaxAmount,
                        FinalPrice = Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero),
                        Subtotal = detail.Subtotal, // Este ya venía redondeado de la BD
                        Status = detail.Status,
                        Description = detail.Description,
                    }
                );
            }

            // 3. Devolvemos el mismo objeto que CreateAsync
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderReference = order.OrderReference,
                CustomerId = order.CustomerId,
                // Totales calculados a partir de los detalles
                Subtotal = Math.Round(totalSubtotal, 2, MidpointRounding.AwayFromZero),
                TotalDiscount = Math.Round(totalDiscount, 2, MidpointRounding.AwayFromZero),
                TotalTax = Math.Round(totalTax, 2, MidpointRounding.AwayFromZero),
                Total = order.Total,
                ExtraData = order.ExtraData,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Details = detailsDto,
            };
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

        private async Task ValidateCustomerAsync(int? customerId)
        {
            // 1. Si es nulo o 0, salimos del método (es un invitado, no hay nada que validar)
            if (!customerId.HasValue || customerId == 0)
            {
                return;
            }

            // 2. Si llegamos aquí, es porque intentaron enviar un ID.
            // Ahora sí verificamos que exista en la base de datos.
            var exists = await _userRepository.ExistsAsync(customerId.Value);

            if (!exists)
            {
                throw new ArgumentException(
                    $"The Customer ID {customerId} was provided but does not exist in our records."
                );
            }
        }
    }
}
