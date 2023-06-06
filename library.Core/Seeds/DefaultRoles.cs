using library.Core.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Archive));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Reception));
            }
                
        }
    }
}
