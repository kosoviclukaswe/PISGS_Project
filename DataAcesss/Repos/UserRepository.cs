using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly PUSGS_ProjectContext _context;

        public UserRepository(PUSGS_ProjectContext context)
        {
            _context = context;
        }

        public bool Create(AppUser user)
        {
            _context.AppUsers.Add(user);
            var writtenEntries = _context.SaveChanges();
            return writtenEntries > 0;
        }

        public AppUser Delete(string id)
        {
            var user = _context.AppUsers.Where(x => x.Id == id)?.ToList()[0];
            if (user != null)
            {
                _context.AppUsers.Remove(user);
                _context.SaveChanges();
            }
            return user;
        }

        public AppUser Read(string id)
        {
            var users = _context.AppUsers.Where(x => x.Id == id).ToList();
            if (users != null && users.Count() > 0)
            {
                return users[0];
            }
            return null;
        }

        public List<AppUser> ReadAll()
        {
            return _context.AppUsers.ToList();
        }

        public AppUser Update(AppUser _user)
        {
            var user = _context.AppUsers.Where(x => x.Id == _user.Id)?.ToList()[0];
            if (user != null)
            {
                user.UserName = _user.UserName;
                user.Email = _user.Email;
                user.Fullname = _user.Fullname;
                user.Address = _user.Address;
                user.Birthdate = _user.Birthdate;
                user.ImagePath = _user.ImagePath;
                
                _context.Update(user);
                _context.SaveChanges();
            }
            return user;
        }
    }
}
