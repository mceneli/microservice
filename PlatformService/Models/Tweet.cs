using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models{
    public class Tweet{
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string ImagePath { get; set; }   
    }
}