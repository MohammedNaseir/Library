using AutoMapper;
using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Users
{
    public class UserService : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var ViewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return ViewModel;
        }
    }
}
