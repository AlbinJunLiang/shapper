using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.Orders;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCart([FromBody] CartRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest(new { message = "El carrito está vacío" });

            var result = await _orderService.ValidateAndCalculateAsync(request.Items);
            return Ok(result);
        }
    }
}
