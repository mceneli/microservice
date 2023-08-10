using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data{
    public interface ITweetRepo{
        bool SaveChanges();
        IEnumerable<Tweet> GetAllTweets();
        Tweet GetTweetById(int id);
        void CreateTweet(Tweet twt);
    }
}