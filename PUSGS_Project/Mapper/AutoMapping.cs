using AutoMapper;
using DataAcesss.Data;
using PUSGS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUSGS_Project.Mapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<SignUpViewModel, ProfileViewModel>();
            CreateMap<AppUser, ProfileViewModel>();
        }
    }
}
