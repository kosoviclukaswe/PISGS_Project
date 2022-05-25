using AutoMapper;
using DataAcesss.Data;
using DataAcesss.Repos.Contracts;
using Microsoft.AspNetCore.Identity;
using ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        public bool Create(AppUserDto user)
        {
            var newUser = _mapper.Map<AppUserDto, AppUser>(user);
            return _userRepository.Create(newUser);
        }

        public AppUserDto Delete(string id)
        {
            var user = _userRepository.Delete(id);
            return _mapper.Map<AppUser, AppUserDto>(user);
        }

        public AppUserDto Read(string id)
        {
            var user = _userRepository.Read(id);
            return _mapper.Map<AppUser, AppUserDto>(user);
        }

        public List<AppUserDto> ReadAll()
        {
            var users = _userRepository.ReadAll();
            return _mapper.Map<List<AppUser>, List<AppUserDto>>(users);
        }

        public AppUserDto Update(AppUserDto user)
        {
            var updatingUser = _mapper.Map<AppUserDto, AppUser>(user);
            _userRepository.Update(updatingUser);
            return user;
        }
    }
}
