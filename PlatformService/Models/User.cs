using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PlatformService.Models{
    public class User{
        [Key]
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public decimal Balance { get; set; }
        public List<UserTweetAccess> AllowedTweetAccess { get; set; } = new List<UserTweetAccess>();
    }
}