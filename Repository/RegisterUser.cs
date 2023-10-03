using BasecampMVC.Constants;
using BasecampMVC.Data;
using BasecampMVC.Interface;
using BasecampMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BasecampMVC.Repository
{
    public class RegisterUser : IRegisterUser
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUser(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Create(RegisterModelInput userModel)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userModel.UserName,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    PhoneNumber = userModel.PhoneNumber,
                    CreatedAt = DateTime.Now,
                };
                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message is {ex.Message}");
                return false;
            }
        }

        public Task<bool> Delete(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserById(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}