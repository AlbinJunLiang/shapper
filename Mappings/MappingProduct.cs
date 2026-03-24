namespace Shapper.Mappings;

using System.Linq.Expressions;
using Shapper.Dtos;
using Shapper.Models;

public static class MappingProduct
{
    public static Expression<Func<Product, ProductStoreViewDto>> ToStoreViewDto =>
        p => new ProductStoreViewDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Discount = p.Discount,
            TaxAmount = p.TaxAmount,
            NewPrice = (p.Price * (1 - p.Discount / 100)) * (1 + p.TaxAmount / 100),
            Quantity = p.Quantity,
            Details = p.Details,
            Status = p.Status,
            SubcategoryName = p.Subcategory.Name,
            Images = p
                .ProductImages.Select(i => new ProductImageDto { Id = i.Id, ImageUrl = i.ImageUrl })
                .ToList(),
        };
}
