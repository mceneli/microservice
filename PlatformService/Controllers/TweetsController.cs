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
using System.Linq;

namespace PlatformService.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase{
        private readonly ITweetRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;
        private readonly IUserRepo _userrepo;

        public TweetsController(ITweetRepo repository,
									IMapper mapper,
									ICommandDataClient commandDataClient,
									IMessageBusClient messageBusClient,
                                    IUserRepo userrepo)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
            _userrepo = userrepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TweetReadDto>> GetTweets(){
            Console.WriteLine("-> Getting Tweets...");
            string authorizatedUser = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            IEnumerable<Tweet> tweetItem = _repository.GetAllTweets();
            List<Tweet> tweetResponse = new();

            foreach (Tweet item in tweetItem)
            {
                User owner = _userrepo.GetUserByName(item.UserName);
                if( (owner.IsPrivateAccount == false) || owner.AllowedTweetAccess.Any(u => u.UserName == authorizatedUser) || (item.UserName == authorizatedUser) ){
                    if(item.ImagePath != null){
                        byte[] imageBytes = System.IO.File.ReadAllBytes(item.ImagePath);
                        string base64String = Convert.ToBase64String(imageBytes);
                        item.ImagePath = base64String;
                    }
                    tweetResponse.Add(item);
                }
 
            }

            return Ok(_mapper.Map<IEnumerable<TweetReadDto>>(tweetResponse));
        }

        [HttpGet("{id}", Name = "GetTweetById")]
        public ActionResult<TweetReadDto> GetTweetById(int id){
            Console.WriteLine("-> Getting Tweet By Id...");

            Tweet tweetItem = _repository.GetTweetById(id);

            if(tweetItem != null){
                return Ok(_mapper.Map<TweetReadDto>(tweetItem));
            }
            return NotFound();
        }

        [HttpGet("GetTweetsByUsername/{username}", Name = "GetTweetsByUsername")]
        public ActionResult<IEnumerable<TweetReadDto>> GetTweetByUsername(string username){
            Console.WriteLine("-> Getting User's Tweets ...");

            string authorizatedUser = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            User user =_userrepo.GetUserByName(username);

            if( (user.IsPrivateAccount == false) || user.AllowedTweetAccess.Any(u => u.UserName == authorizatedUser) || authorizatedUser == username ){
                IEnumerable<Tweet> tweetItem = _repository.GetTweetsByUsername(username);
                foreach (Tweet item in tweetItem)
                {
                    if(item.ImagePath != null){
                        byte[] imageBytes = System.IO.File.ReadAllBytes(item.ImagePath);
                        string base64String = Convert.ToBase64String(imageBytes);
                        item.ImagePath = base64String;
                    }
                }
                return Ok(_mapper.Map<IEnumerable<TweetReadDto>>(tweetItem));
            }
            return BadRequest("Profile is Hidden");         
        }

        [HttpPost("CreateTweet"), Authorize(Roles = "User")]
        public bool CreateTweet([FromForm] TweetCreateDto tweetCreateDto){
            Console.WriteLine("-> Creating Tweet...");

            string authorizatedUser = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            Tweet tweetModel = _mapper.Map<Tweet>(tweetCreateDto);
            tweetModel.Date = DateTime.Now;

            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files[0];  
                string imagePath = SaveImageToStorage(imageFile,authorizatedUser);
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

        private string SaveImageToStorage(IFormFile imageFile,string authorizatedUser)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", authorizatedUser);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", authorizatedUser, uniqueFileName);
        }
    }
}