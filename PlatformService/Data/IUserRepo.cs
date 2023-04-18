using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data{
    public interface IUserRepo{
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        void CreateUser(User user);
    }
}