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
    public class UsersController : ControllerBase{
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;


        public UsersController(IUserRepo repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        [HttpGet]
        public ActionResult<IEnumerable<UserReadDto>> GetUsers(){
            Console.WriteLine("-> Getting Users...");

            IEnumerable<User> userItem = _repository.GetAllUsers();

            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(userItem));
        }

        [HttpPost("MakePrivate"), Authorize(Roles = "User")]
        public bool MakePrivate([FromBody] MakePrivateRequest request){
            Console.WriteLine("-> Account Making Private...");

            bool isPrivate = request.IsPrivate;

            string authorizatedUser = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            _repository.MakePrivate(authorizatedUser,isPrivate);
            _repository.SaveChanges();
            return true;
        }
        
    }
}