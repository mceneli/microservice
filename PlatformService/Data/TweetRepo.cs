using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data{
    public class TweetRepo : ITweetRepo
    {
        private readonly AppDbContext _context;

        public TweetRepo(AppDbContext context){
            _context = context;
        }
        public void CreateTweet(Tweet twt)
        {
            if(twt == null){
                throw new ArgumentNullException(nameof(twt));
            }
            _context.Tweets.Add(twt);
        }

        public IEnumerable<Tweet> GetAllTweets()
        {
            return _context.Tweets.ToList();
        }

        public Tweet GetTweetById(int id)
        {
            return _context.Tweets.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Tweet> GetTweetsByUsername(string username)
        {
            return _context.Tweets.ToList().Where(p => p.UserName == username);
        }

        public void DeleteTweet(Tweet twt)
        {
            if (twt == null)
            {
                throw new ArgumentNullException(nameof(twt));
            }

            _context.Tweets.Remove(twt);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >=0);
        }
    }
}