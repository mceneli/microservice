using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models{
    public class Subscription
        {
            [Key]
            [Required]
            public int Id { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Subscriber { get; set; }
        }
}