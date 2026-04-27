using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Categories;
using Shapper.Dtos.Faqs;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Dtos.Locations;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Dtos.ProductImages;
using Shapper.Dtos.Products;
using Shapper.Dtos.Reviews;
using Shapper.Dtos.Roles;
using Shapper.Dtos.Store;
using Shapper.Dtos.StoreLinks;
using Shapper.Dtos.Subcategories;
using Shapper.Dtos.Users;
using Shapper.Models;

namespace Shapper.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos existentes...
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserResponseDto>();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, RoleResponseDto>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryResponseDto>().ReverseMap();
            CreateMap<Subcategory, SubcategoryResponseDto>().ReverseMap();
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();

            // ✅ NUEVO: Mapeos para ProductImage
            CreateMap<ProductImage, ProductImageResponseDto>()
                .ReverseMap();

            CreateMap<CreateProductImageDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateProductImageDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore());

            // ========== MAPEOS DE PRODUCTO CENTRALIZADOS ==========

            // Product -> ProductResponseDto
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            // ProductDto -> Product
            CreateMap<ProductDto, Product>();

            // Product -> ProductStoreViewDto (con cálculo de NewPrice)
            CreateMap<Product, ProductStoreViewDto>()
                .ForMember(
                    dest => dest.NewPrice,
                    opt =>
                        opt.MapFrom(src =>
                            (src.Price * (1 - src.Discount / 100)) * (1 + src.TaxAmount / 100)
                        )
                )
                .ForMember(
                    dest => dest.SubcategoryName,
                    opt => opt.MapFrom(src => src.Subcategory != null ? src.Subcategory.Name : null)
                )
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            // Product -> ProductStoreView2Dto
            CreateMap<Product, ProductStoreView2Dto>()
                .ForMember(
                    dest => dest.NewPrice,
                    opt =>
                        opt.MapFrom(src =>
                            (src.Price * (1 - src.Discount / 100)) * (1 + src.TaxAmount / 100)
                        )
                )
                .ForMember(
                    dest => dest.SubcategoryName,
                    opt => opt.MapFrom(src => src.Subcategory != null ? src.Subcategory.Name : null)
                )
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            // ProductImage mapeos existentes
            CreateMap<ProductImage, ProductImageDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));

            CreateMap<ProductImage, ProductImageDto2>();

            // Otros mapeos existentes...
            CreateMap<Product, ProductFilterDto>().ReverseMap();
            CreateMap<Order, OrderResponseDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailResponseDto>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
            CreateMap<Faq, FaqResponseDto>().ReverseMap();
            CreateMap<Faq, FaqDto>().ReverseMap();
            CreateMap<FeaturedProduct, FeaturedProductResponseDto>().ReverseMap();
            CreateMap<FeaturedProduct, FeaturedProductDto>().ReverseMap();
            CreateMap<OrderPayment, OrderPaymentResponseDto>().ReverseMap();
            CreateMap<OrderPayment, OrderPaymentDto>().ReverseMap();
            CreateMap<Review, ReviewResponseDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Review, ReviewResponse2Dto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ReverseMap();

            // Store mapeos
            CreateMap<Store, StoreResponseDto>()
                .ForMember(dest => dest.StoreLinks, opt => opt.MapFrom(src => src.StoreLinks));

            CreateMap<StoreDto, Store>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.StoreLinks, opt => opt.Ignore());

            CreateMap<Location, LocationResponseDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();

            // StoreLink mapeos
            CreateMap<StoreLink, StoreLinkResponseDto>().ReverseMap();

            CreateMap<StoreLinkDto, StoreLink>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore());


        }
    }
}