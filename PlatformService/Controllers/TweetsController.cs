using System;
using System.IO;
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
using Microsoft.AspNetCore.Http;

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

        [HttpPost("CreateTweet"), Authorize(Roles = "User")]
        public bool CreateTweet([FromForm] TweetCreateDto tweetCreateDto){
            Console.WriteLine("-> Creating Tweet...");

            var tweetModel = _mapper.Map<Tweet>(tweetCreateDto);
            tweetModel.Date = DateTime.Now;

            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files[0];  
                string imagePath = SaveImageToStorage(imageFile);
                tweetModel.ImagePath = imagePath;
            }

            _repository.CreateTweet(tweetModel);
            _repository.SaveChanges();
            return true;
        }

        [HttpDelete("{id}", Name = "DeleteTweetById"), Authorize(Roles = "User")]
        public ActionResult DeleteTweetById(int id)
        {
            Console.WriteLine("-> Deleting Tweet By Id...");
            string authorizatedUser = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            Tweet tweetItem = _repository.GetTweetById(id);

            if (tweetItem == null)
            {
                return NotFound();
            }
            if(authorizatedUser == tweetItem.UserName){
                _repository.DeleteTweet(tweetItem);
                _repository.SaveChanges();
                return Ok();
            }          
            
            CustomError error = new CustomError
            {
                Message = "Tweet does not belong to the user",
                StatusText = "Custom Bad Request"
            };

            return BadRequest(error);
        }

        private string SaveImageToStorage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return Path.Combine("/uploads", uniqueFileName);
        }
    }
}