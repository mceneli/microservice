using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context){
            _context = context;
        }
        public void CreateUser(User user)
        {
            if(user == null){
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Add(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >=0);
        }

        public User GetUserByName(string username)
        {
            return _context.Users.FirstOrDefault(p => p.Username == username);
        }

        public bool IsThereUser(string username)
        {
            var user = _context.Users.FirstOrDefault(p => p.Username == username);
            if(user != null){
                return true;
            }
            return false;
        }

    }
}