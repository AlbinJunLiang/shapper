using Microsoft.AspNetCore.Mvc;
using Shapper.Services.StoreFront;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreFrontController : ControllerBase
    {
        private readonly IStoreFrontService _storeFrontService;
        private readonly ILogger<StoreFrontController> _logger;

        public StoreFrontController(
            IStoreFrontService storeFrontService,
            ILogger<StoreFrontController> logger)
        {
            _storeFrontService = storeFrontService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los datos para la página principal de la tienda
        /// </summary>
        [HttpGet("home-data/{storeCode}")]
        public async Task<IActionResult> GetHomeData(
            [FromRoute] string storeCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool featured = true,
            [FromQuery] int categoriesPage = 1,
            [FromQuery] int categoriesPageSize = 8)
        {
            try
            {
                var result = await _storeFrontService.GetHomeDataAsync(
                    storeCode, page, pageSize, featured, categoriesPage, categoriesPageSize);

                if (result.StoreInfo == null)
                    return NotFound(new { success = false, message = $"Store with code '{storeCode}' not found" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store home data for store {StoreCode}", storeCode);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}