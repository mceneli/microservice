using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos{
    public class TweetCreateDto{
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}