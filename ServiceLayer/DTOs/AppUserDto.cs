using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class AppUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string ImagePath { get; set; }
    }
}
