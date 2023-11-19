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
        private static User user1 = new();
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userrepository;
        private readonly ILogger _logger;

        public static User User1 { get => user1; set => user1 = value; }

        public AuthController(IConfiguration configuration,IMapper mapper,IUserRepo userrepository,ILogger logger)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userrepository = userrepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public ActionResult<UserDto> Register(UserDto request){
            Console.WriteLine("-> Registering User...");
            CreatePasswordHash(request.Password,out byte[] passwordHash,out byte[] passwordSalt);

            if(_userrepository.IsThereUser(request.Username)){
                var resulttoken = new { result = "1" };
                return new JsonResult(resulttoken);
            }

            User1.Username = request.Username;
            User1.PasswordHash = passwordHash;
            User1.PasswordSalt = passwordSalt;

            User userModel = _mapper.Map<User>(User1);
            _userrepository.CreateUser(userModel);
            _userrepository.SaveChanges();
            
            _logger.Information("registered");
            _logger.Error("registered");

            return Ok(User1);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request){
            Console.WriteLine("-> Logging In User...");

            User user = _userrepository.GetUserByName(request.Username);

            if(user == null || user.Username != request.Username){
                var resulttoken = new { result = "1" };
                return new JsonResult(resulttoken);
            }
            if(!VerifyPasswordHash(request.Password,user.PasswordHash)){
                var resulttoken = new { result = "2" };
                return new JsonResult(resulttoken);
            }

            string token = CreateToken(user);

            var jsontoken = new { result = "3", Token = token };
            return new JsonResult(jsontoken);
        }

        private string CreateToken(User user){
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials creds = new(key,SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims:claims,
                expires: System.DateTime.Now.AddDays(1),
                signingCredentials:creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt){
            using HMACSHA512 hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password,byte[] passwordHash){
            using HMACSHA512 hmac = new(User1.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

    }
}
