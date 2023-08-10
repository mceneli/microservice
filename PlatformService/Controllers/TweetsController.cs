using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using Microsoft.AspNetCore.Authorization;

namespace PlatformService.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase{
        private readonly ITweetRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;

        public TweetsController(ITweetRepo repository,
									IMapper mapper,
									ICommandDataClient commandDataClient,
									IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
        }

        //[HttpGet, Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult<IEnumerable<TweetReadDto>> GetTweets(){
            Console.WriteLine("-> Getting Tweets...");

            var tweetItem = _repository.GetAllTweets();

            return Ok(_mapper.Map<IEnumerable<TweetReadDto>>(tweetItem));
        }

        [HttpGet("{id}", Name = "GetTweetById")]
        public ActionResult<TweetReadDto> GetTweetById(int id){
            Console.WriteLine("-> Getting Tweet By Id...");

            var tweetItem = _repository.GetTweetById(id);

            if(tweetItem != null){
                return Ok(_mapper.Map<TweetReadDto>(tweetItem));
            }
            return NotFound();
        }

        [HttpPost]
        public async void CreateTweet(TweetCreateDto tweetCreateDto){
            Console.WriteLine("-> Creating Tweet...");

            tweetCreateDto.Date = DateTime.Now;

            var tweetModel = _mapper.Map<Tweet>(tweetCreateDto);
            _repository.CreateTweet(tweetModel);
            _repository.SaveChanges();
        }

    }
}