using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos{
    public class SubscriptionCreateDto{
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Subscriber { get; set; }
    }
}