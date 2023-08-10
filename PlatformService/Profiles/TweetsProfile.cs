using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles{
    public class TweetsProfile : Profile {
        public TweetsProfile()
        {
			// Source -> Target
            CreateMap<Tweet, TweetReadDto>();
            CreateMap<TweetCreateDto, Tweet>();
        }
    }
}