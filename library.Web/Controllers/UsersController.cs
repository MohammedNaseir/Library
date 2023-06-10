using library.Data.Models;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace library.Web.Controllers
{
    [Authorize(Roles =AppRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly IUser _userService;

    
        public UsersController(IUser userService)
        {
            _userService = userService;
        }

        public  IActionResult Index()
        {
            var users =  _userService.GetUsers();
            return View(users);
        }
    }
}
