using AutoMapper;
using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Users
{
    public class UserService : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
   
        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }
  
        public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var ViewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return ViewModel;
        }
  
        public async Task<List<SelectListItem>> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Select(r=> new SelectListItem
                {
                    Text= r.Name,
                    Value =r.Name
                }).ToListAsync();
            return roles;
        }
   
        public async Task<(UserViewModel, List<string>,string? code,ApplicationUser? user)> Create(UserFormViewModel model, string createdById)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                FullName = model.FullName,
                UserName = model.UserName,
                CreatedById = createdById
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                return (_mapper.Map<UserViewModel>(user), null,code,user);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (null, errors,"",null);
            }
        }
        public async Task<bool> IsUsernameExists(UserViewModel user)
        {
            var appuser = await _userManager.FindByNameAsync(user.Username);
            bool isAllowed = appuser is null || appuser.Id.Equals(user.Id);
            return isAllowed;
        }
        public async Task<bool> IsEmailExists(UserViewModel user)
        {
            var appuser =  await _userManager.FindByEmailAsync(user.Email);
            bool isAllowed = appuser is null || appuser.Email.Equals(user.Email);
            return isAllowed;
        }
     
        public async Task<ApplicationUser> GetUser(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
  
        public async Task<IdentityResult> UpdateResult(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
      
        public async Task UpdateAsync(ApplicationUser user)
        {         
            await _userManager.UpdateAsync(user);
        }
    
        public async Task<IdentityResult> ChangePassAsync(ApplicationUser user,string newpass)
        {
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user,newpass);
            return result;
        }
 
        public UserViewModel MapToUserViewModel(ApplicationUser user)
        {
            return _mapper.Map<UserViewModel>(user);
        }
    
        public ApplicationUser MapToApplicationUser(UserFormViewModel model ,ApplicationUser user)
        {
            return _mapper.Map(model,user);
        }
   
        public UserFormViewModel MapToUserForm(ApplicationUser user)
        {
            return _mapper.Map<UserFormViewModel>(user);
        }
      
        public async Task<IList<string>> GetRolesList(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
      
        public async Task RemoveFromRole(ApplicationUser user,IList<string> currentRoles)
        {
            await _userManager.RemoveFromRolesAsync(user,currentRoles);
        }
      
        public async Task AddToRole(ApplicationUser user, IList<string> selectedRoles)
        {
            await _userManager.AddToRolesAsync(user,selectedRoles);
        }
        public async Task Unlock(ApplicationUser user)
        {
            var isLocked = await _userManager.IsLockedOutAsync(user);
            if(isLocked)
               await _userManager.SetLockoutEndDateAsync(user, null);             
        }
    }
}