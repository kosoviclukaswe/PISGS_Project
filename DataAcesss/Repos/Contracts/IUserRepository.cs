using DataAcesss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Repos.Contracts
{
    public interface IUserRepository
    {
        bool Create(AppUser user);
        AppUser Read(string id);
        List<AppUser> ReadAll();
        AppUser Update(AppUser user);
        AppUser Delete(string id);
    }
}
