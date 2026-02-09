using AutoMapper;
using Shapper.DTOs;
using Shapper.Models;

namespace Shapper.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
