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
        }
    }
}
