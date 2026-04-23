using Shapper.Dtos.ProductImages;

namespace Shapper.Services.ProductImages
{
    public interface IProductImageService
    {
        Task<ProductImageResponseDto?> GetByIdAsync(int id);
        Task<List<ProductImageResponseDto>> GetByProductIdAsync(int productId);
        Task<ProductImageResponseDto> CreateAsync(CreateProductImageDto dto);
        Task<ProductImageResponseDto> UpdateAsync(int id, UpdateProductImageDto dto);
        Task<bool> DeleteAsync(int id, string provider);
    }
}