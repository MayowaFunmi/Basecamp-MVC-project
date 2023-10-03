using BasecampMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BasecampMVC.Constants
{
    public class SeedRoles
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Owner.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));

            // create owner
            var user = new ApplicationUser
            {
                UserName = "mayowafunmi",
                Email = "akinade.mayowa@gmail.com",
                FirstName = "Mayowa",
                LastName = "Akinade",
                EmailConfirmed= true,
                PhoneNumber = "08187669362",
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Treasure@2020");
                await userManager.AddToRolesAsync(user, new [] {Roles.Owner.ToString(), Roles.User.ToString()});
            }
        }
    }
}