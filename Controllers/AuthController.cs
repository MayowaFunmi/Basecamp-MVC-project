using BasecampMVC.Data;
using BasecampMVC.Interface;
using BasecampMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasecampProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly IRegisterUser _registerUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IRegisterUser registerUser, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _registerUser = registerUser;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegisterUser(RegisterModelInput userModel)
        {
            if (ModelState.IsValid)
            {
                var regUser = await _registerUser.Create(userModel);
                if (regUser)
                {
                    TempData["msg"] = "User registered successfully";
                    return RedirectToAction("LoginUser");
                }
                else
                {
                    TempData["msg"] = "User registration failed";
                    return View();
                }
            }
            return View(userModel);
        }

        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(LoginModelInput modelInput)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(modelInput.UserName);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, modelInput.Password, isPersistent: true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Login Successful";
                        return RedirectToAction("Index", "Home");
                    }
                    TempData["message"] = "Failed to login user";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View();
                }
                TempData["message"] = "User not found";
                return View();
            }
            else
            {
                TempData["message"] = "Invalid Form Data";
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> LogoutUser()
        {
            await _signInManager.SignOutAsync();
            TempData["message"] = "You are logged out";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            // get user, 
            var user = await _registerUser.GetUserById(userId);
            if (user != null)
            {
                var availableRoles = await _roleManager.Roles.ToListAsync(); // all roles
                var userRoles = await _userManager.GetRolesAsync(user); // user specific roles
                var viewModel = new UserViewModelData
                {
                    //User = user, 
                    Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, PhoneNumber = user.PhoneNumber,
                    AvailableRoles = availableRoles, CreatedAt = user.CreatedAt,
                    UserRoles = userRoles
                };

                return View(viewModel);
            }
            TempData["msg"] = "User not found";
            return View();
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleToUser(UserViewModelData modelData)
        {
            if (ModelState.IsValid)
            {
                var user = await _registerUser.GetUserById(modelData.Id);
                var userRoles = await _userManager.GetRolesAsync(user);
                bool roleExists = userRoles.Contains(modelData.SelectedRole);
                if (!roleExists)
                {
                    var roleAdded = await _userManager.AddToRolesAsync(user, new[] { modelData.SelectedRole });
                    if (roleAdded.Succeeded)
                    {
                        TempData["msg"] = $"Role '{modelData.SelectedRole}' added to user";
                        return RedirectToAction("GetUserById", new { userId = modelData.Id });
                    }
                    TempData["msg"] = "Failed to add role to user";
                    return RedirectToAction("GetUserById", new { userId = modelData.Id });
                }
                TempData["msg"] = $"User already has the Role '{modelData.SelectedRole}'";
                return RedirectToAction("GetUserById", new { userId = modelData.Id });
            }
            
            TempData["msg"] = "Invalid form state";
            // foreach (var modelState in ModelState.Values)
            // {
            //     foreach (var error in modelState.Errors)
            //     {
            //         // Log or display the error message
            //         TempData["msg"] = error.ErrorMessage;
            //         Console.WriteLine(error.ErrorMessage);
            //     }
            // }
            return RedirectToAction("GetUserById", new { userId = modelData.Id });
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoleFromUser(UserViewModelData modelData)
        {
            if (ModelState.IsValid)
            {
                var user = await _registerUser.GetUserById(modelData.Id);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, modelData.SelectedRole))
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, modelData.SelectedRole);
                        if (result.Succeeded)
                        {
                            TempData["msg"] = $"Role '{modelData.SelectedRole}' removed from user";
                            return RedirectToAction("GetUserById", new { userId = modelData.Id });
                        }
                        TempData["msg"] = "Failed to remove user from role";
                        return RedirectToAction("GetUserById", new { userId = modelData.Id });
                    }
                    TempData["msg"] = $"User does not have the Role '{modelData.SelectedRole}'";
                    return RedirectToAction("GetUserById", new { userId = modelData.Id });
                }
                TempData["msg"] = "User not found!";
                return RedirectToAction("GetUserById", new { userId = modelData.Id });
            }
            TempData["msg"] = "Invalid form state";
            return RedirectToAction("GetUserById", new { userId = modelData.Id });
        }
    }
}