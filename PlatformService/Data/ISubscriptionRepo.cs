using System.Collections.Generic;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Data{
    public interface ISubscriptionRepo{
        bool SaveChanges();
        IEnumerable<Subscription> GetSubscribersByUsername(string username);
        void Subscribe(Subscription sub);
        bool CheckSubscription(string username, string subscriber);
    }
}