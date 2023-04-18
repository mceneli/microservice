using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models{
    public class User{
        [Key]
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
    }
}