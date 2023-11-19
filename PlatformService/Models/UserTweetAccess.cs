using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models{
    public class UserTweetAccess
        {
            [Key]
            public int Id { get; set; }
            public string UserName { get; set; }
        }
}