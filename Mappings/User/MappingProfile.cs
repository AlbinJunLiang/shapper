using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Para crear usuario
            CreateMap<CreateUserDto, User>();

            // Para responder usuario
            CreateMap<User, UserResponseDto>();

            // Roles si los tienes
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, RoleResponseDto>().ReverseMap();
            CreateMap<Contact, ContactDto>().ReverseMap();
            CreateMap<Contact, ContactResponseDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Category, CategoryResponseDto>().ReverseMap();

            CreateMap<Subcategory, SubcategoryResponseDto>().ReverseMap();
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();

            CreateMap<Product, ProductResponseDto>().ReverseMap();
            CreateMap<Product, ProductStoreViewDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();

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

            CreateMap<StoreInformation, StoreInformationResponseDto>().ReverseMap();
            CreateMap<StoreInformation, StoreInformationDto>().ReverseMap();

            CreateMap<Location, LocationResponseDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
        }
    }
}
