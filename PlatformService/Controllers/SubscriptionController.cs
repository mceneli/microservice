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
    public class SubscriptionController : ControllerBase{
        private readonly ISubscriptionRepo _repository;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userrepo;

        public SubscriptionController(ISubscriptionRepo repository,
									IMapper mapper,
                                    IUserRepo userrepo)
        {
            _repository = repository;
            _mapper = mapper;
            _userrepo = userrepo;
        }
 
        [HttpPost("Subscribe"), Authorize(Roles = "User")]
        public ActionResult Subscribe([FromBody] SubscriptionRequest request){
            Console.WriteLine("-> Subscribing ...");
            string subscriberName = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            string username = request.username;

            Subscription sub = new(){ UserName=username, Subscriber=subscriberName };

            User user = _userrepo.GetUserByName(username);
            User subscriber = _userrepo.GetUserByName(subscriberName);

            if(!_repository.CheckSubscription(username, subscriberName) && subscriber.Balance>=user.Fee){
                subscriber.Balance-=user.Fee;
                user.Balance+=user.Fee;
                _repository.Subscribe(sub);
                _userrepo.SaveChanges();
                _repository.SaveChanges();
                return Ok();
            }
            CustomError error = new()
            {
                Message = subscriber.Balance<user.Fee ? "insufficient funds" : "already subscribed",
                StatusText = "Custom Bad Request"
            };

            return BadRequest(error);
        }
       
    }
}