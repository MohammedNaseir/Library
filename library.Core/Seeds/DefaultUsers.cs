using library.Core.Constants;
using library.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = new()
            {
                UserName="admin",
                Email="admin@library.com",
                EmailConfirmed=true,
                FullName="Admin Mohammed",
                
            };
            var user = await userManager.FindByEmailAsync(admin.Email);
            if(user == null)
            {
                await userManager.CreateAsync(admin,"P@ssword123");
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }
    }
}
