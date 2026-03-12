using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.OrderDetails;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDetailDto dto)
        {
            try
            {
                var orderDetail = await _orderDetailService.CreateAsync(dto);
                return Ok(orderDetail);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The specified subcategory does not exist."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var orderDetail = await _orderDetailService.GetByIdAsync(id);
            if (orderDetail == null)
                return NotFound();
            return Ok(orderDetail);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(
                    new { success = false, message = "Page number must be greater than 0." }
                );

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(
                    new { success = false, message = "Page size must be between 1 and 100." }
                );

            var result = await _orderDetailService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var orderDetail = await _orderDetailService.UpdateAsync(id, dto);

                return Ok(new { message = "OrderDetail updated successfully", data = orderDetail });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "OrderDetail not found." => NotFound(
                        new { message = ex.Message, status = 404 }
                    ),
                    "OrderDetail name already exists." => Conflict(
                        new { message = ex.Message, status = 409 }
                    ),

                    _ => StatusCode(500, new { message = "Internal server error." }),
                };
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _orderDetailService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
