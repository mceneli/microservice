using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles{
    public class UsersProfile : Profile {
        public UsersProfile()
        {
			// Source -> Target
            CreateMap<UserDto, User>();

        }
    }
}