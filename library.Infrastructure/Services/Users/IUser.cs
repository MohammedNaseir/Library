using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Users
{
    public interface IUser
    {
        public IEnumerable<UserViewModel> GetUsers();
        public Task<List<SelectListItem>> GetRoles();
        //public Task<UserViewModel> Create(UserFormViewModel model, string createdById);
        public Task<(UserViewModel, List<string>)> Create(UserFormViewModel model, string createdById);

        public Task<bool> IsUsernameExists(UserViewModel user);
        public Task<bool> IsEmailExists(UserViewModel user);
        public Task<ApplicationUser> GetUser(string id);
        public Task UpdateAsync(ApplicationUser user);
        public Task<IdentityResult> ChangePassAsync(ApplicationUser user, string newpass);
        public UserViewModel MapToUserViewModel(ApplicationUser user);
        public UserFormViewModel MapToUserForm(ApplicationUser user);
        public Task<IList<string>> GetRolesList(ApplicationUser user);
        public Task RemoveFromRole(ApplicationUser user, IList<string> currentRoles);
        public Task AddToRole(ApplicationUser user, IList<string> selectedRoles);

        public ApplicationUser MapToApplicationUser(UserFormViewModel model, ApplicationUser user);
        public Task<IdentityResult> UpdateResult(ApplicationUser user);


    }
}
