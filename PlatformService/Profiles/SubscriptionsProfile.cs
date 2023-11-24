using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles{
    public class SubscriptionsProfile : Profile {
        public SubscriptionsProfile()
        {
			// Source -> Target
            CreateMap<SubscriptionCreateDto, Subscription>();
        }
    }
}