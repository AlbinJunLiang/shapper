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
        }
    }
}
