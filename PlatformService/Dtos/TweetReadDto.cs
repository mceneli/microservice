using System;

namespace PlatformService.Dtos{
    public class TweetReadDto{
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}