using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Contracts
{
    public interface IUserService
    {
        bool Create(AppUserDto user);
        AppUserDto Read(string id);
        List<AppUserDto> ReadAll();
        AppUserDto Update(AppUserDto user);
        AppUserDto Delete(string id);
    }
}
