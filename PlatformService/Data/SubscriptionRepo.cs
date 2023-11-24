using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Data{
    public class SubscriptionRepo : ISubscriptionRepo
    {
        private readonly AppDbContext _context;

        public SubscriptionRepo(AppDbContext context){
            _context = context;
        }
       
        public IEnumerable<Subscription> GetSubscribersByUsername(string username)
        {
            return _context.Subscriptions.Where(p => p.UserName == username).ToList();
        }

        public void Subscribe(Subscription sub)
        {
            _context.Subscriptions.Add(sub);
        }

        public bool CheckSubscription(string username, string subscriber){
            return GetSubscribersByUsername(username).Any(s => s.Subscriber == subscriber);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >=0);
        }
        
    }
}