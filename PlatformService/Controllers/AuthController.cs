using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using Serilog;

namespace PlatformService.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase{
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repository;
        private readonly IUserRepo _userrepository;
        private readonly ILogger _logger;

        public AuthController(IConfiguration configuration,IMapper mapper,IPlatformRepo repository,IUserRepo userrepository,ILogger logger)
        {
            _configuration = configuration;
            _mapper = mapper;
            _repository = repository;
            _userrepository = userrepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserDto request){
            Console.WriteLine("-> Registering User...");
            CreatePasswordHash(request.Password,out byte[] passwordHash,out byte[] passwordSalt);

            if(_userrepository.IsThereUser(request.Username)){
                var resulttoken = new { result = "1" };
                return new JsonResult(resulttoken);
            }

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var userModel = _mapper.Map<User>(user);
            _userrepository.CreateUser(userModel);
            _userrepository.SaveChanges();
            
            _logger.Information("registered");
            _logger.Error("csac");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request){
            Console.WriteLine("-> Logging In User...");

            var user = _userrepository.GetUserByName(request.Username);

            if(user == null || user.Username != request.Username){
                var resulttoken = new { result = "1" };
                return new JsonResult(resulttoken);
            }
            if(!VerifyPasswordHash(request.Password,user.PasswordHash,user.PasswordSalt)){
                var resulttoken = new { result = "2" };
                return new JsonResult(resulttoken);
            }

            string token = CreateToken(user);

            var jsontoken = new { result = "3", Token = token };
            return new JsonResult(jsontoken);
        }

        private string CreateToken(User user){
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims:claims,
                expires: System.DateTime.Now.AddDays(1),
                signingCredentials:creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt){
            using(var hmac = new HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password,byte[] passwordHash,byte[] passwordSalt){
            using(var hmac = new HMACSHA512(user.PasswordSalt)){
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
