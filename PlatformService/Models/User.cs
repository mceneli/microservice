using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PlatformService.Data;

namespace PlatformService.Models{
    public class User{
        [Key]
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public decimal Balance { get; set; } = 5;
        public bool IsPrivateAccount { get; set; } = false;
        public decimal Fee { get; set; } = 1;
    }
}