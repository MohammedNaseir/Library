using CloudinaryDotNet.Actions;
using library.Core.Constants;
using library.Data.Models;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel
            {
                Roles= await _userService.GetRoles()
            };
            return PartialView("_Form", viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if(!ModelState.IsValid)            
                return BadRequest();
            var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var (viewModel,errors) = await _userService.Create(model, createdBy);
            if(errors != null)
            {
                return BadRequest(string.Join(", ", errors));
            }
            return PartialView("_UserRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return NotFound();
            var viewModel = new ResetPasswordFormViewModel { Id = user.Id };
            return PartialView("_ResetPasswordForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userService.GetUser(model.Id);
            if (user == null)
                return NotFound();
            var currentPasswordHash = user.PasswordHash;
            var result = await _userService.ChangePassAsync(user,model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                user.LastUpdatedOn= DateTime.Now;
                await _userService.UpdateAsync(user);
                var viewModel = _userService.MapToUserViewModel(user);
                return PartialView("_UserRow", viewModel);

            }
            user.PasswordHash = currentPasswordHash;
            await _userService.UpdateAsync(user);   
            return PartialView("_UserRow", string.Join(", ", result.Errors.Select(e=>e.Description)));
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return NotFound();
            var viewModel = _userService.MapToUserForm(user);
            viewModel.SelectedRoles = await _userService.GetRolesList(user);
            viewModel.Roles = await _userService.GetRoles();
            return PartialView("_Form", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userService.GetUser(model?.Id);
            
            if (user == null)
                return NotFound();
            
            user = _userService.MapToApplicationUser(model,user);
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdatedOn = DateTime.Now;

            var result = await _userService.UpdateResult(user);
            if (result.Succeeded)
            {
                var cuurrentRoles = await _userService.GetRolesList(user);
                var rolesUpdated = !cuurrentRoles.SequenceEqual(model.SelectedRoles);
                if (rolesUpdated)
                {
                    await _userService.RemoveFromRole(user, cuurrentRoles);
                    await _userService.AddToRole(user, model.SelectedRoles);
                }
                var viewModel = _userService.MapToUserViewModel(user);
                return PartialView("_UserRow", viewModel);
            }
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }



        public async Task<IActionResult> AllowUsername(UserViewModel model)
        {
            bool isAllowed = await _userService.IsUsernameExists(model);
            //var isAllowed = user is null || user.UserName.Equals(model.Username);
            return Json(isAllowed);
        }
        public async Task<IActionResult> AllowEmail(UserViewModel model)
        {
            bool isAllowed = await _userService.IsEmailExists(model);
            //var isAllowed = user is null || user.Email.Equals(model.Email);
            return Json(isAllowed);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            ApplicationUser user = await _userService.GetUser(id);
            if (user is null)
                return NotFound();
            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _userService.UpdateAsync(user);
            return Ok(user.LastUpdatedOn.ToString());
        }


    }
}
