using BasecampMVC.Data;
using BasecampMVC.Interface;
using BasecampMVC.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasecampMVC.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly Cloudinary _cloudinary;

        public ProjectController(UserManager<ApplicationUser> userManager, IProjectRepository projectRepository, Cloudinary cloudinary)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _cloudinary = cloudinary;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userProjects = await _projectRepository.GetProjectByUserId(user.Id);
                if (userProjects != null)
                {
                    return View(userProjects);
                }
                TempData["msg"] = "You don't have any project";
                return View();
            }
            TempData["msg"] = "User not found";
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectViewModel projectViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (projectViewModel.ImageFile != null)
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(projectViewModel.ImageFile.Name, projectViewModel.ImageFile.OpenReadStream())
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                        // Save the image URL to the project model
                        projectViewModel.Url = uploadResult.Url.ToString();
                    }
                    var result = await _projectRepository.Create(projectViewModel, user);
                    if (result)
                    {
                        TempData["msg"] = "Project created successfully";
                        return RedirectToAction("Index");
                    }
                    TempData["msg"] = "Failed to create Project";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "User not found";
                return RedirectToAction("Index");
            }
            TempData["msg"] = "Invalid form state";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string projectId)
        {
            var detail = await _projectRepository.GetProjectById(projectId);
            if (detail != null)
            {
                return View(detail);
            }
            TempData["msg"] = "Project Detail not found";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(EditProjectViewModel editProjectViewModel)
        {
            if (ModelState.IsValid)
            {
                var project = await _projectRepository.Update(editProjectViewModel);
                if (project)
                {
                    TempData["msg"] = "Project updated successfully";
                    return RedirectToAction("Detail", new { editProjectViewModel.ProjectId});
                }
                TempData["msg"] = "Failed to update project";
                return RedirectToAction("Detail", new { editProjectViewModel.ProjectId});
            }
            TempData["msg"] = "Invalid form state";
            return RedirectToAction("Detail", new { editProjectViewModel.ProjectId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteProjectModel deleteProjectModel)
        {
            if (!string.IsNullOrEmpty(deleteProjectModel.Id))
            {
                var result = await _projectRepository.Delete(deleteProjectModel.Id);
                if (result)
                {
                    TempData["msg"] = "Project deleted successfully";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "Failed to delete project";
                return RedirectToAction("Detail", new { deleteProjectModel.Id});
            }
            TempData["msg"] = "Invalid project id";
            return RedirectToAction("Detail", new { deleteProjectModel.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMembers(string email, string projectId)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var project = await _projectRepository.GetProjectById(projectId);
            if (user != null && project != null)
            {
                var result = await _projectRepository.AddTeamMembers(project.ProjectId.ToString(), user.Id);
                if (result)
                {
                    TempData["msg"] = "User added to team successfully";
                    return RedirectToAction("GetTeamMembers", new { projectId});
                }
                TempData["msg"] = "Failed to add user to team";
                return RedirectToAction("Detail", new { projectId});
            }
            TempData["msg"] = "user or project not found!!";
            return RedirectToAction("Detail", new { projectId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTeamMember(string userId, string projectId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var project = await _projectRepository.GetProjectById(projectId);
            if (user != null && project != null)
            {
                var result = await _projectRepository.RemoveTeamMember(project.ProjectId.ToString(), user.Id);
                if (result)
                {
                    TempData["msg"] = "User removed from team successfully";
                    return RedirectToAction("GetTeamMembers", new { projectId});
                }
                TempData["msg"] = "Failed to remove user from team";
                return RedirectToAction("Detail", new { projectId});
            }
            TempData["msg"] = "user or project not found!!";
            return RedirectToAction("Detail", new { projectId});
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(string projectId)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            if (project != null)
            {
                List<ApplicationUser> members = await _projectRepository.ProjectTeamMembers(project);
                ProjectTeamMembersModel teamMembersModel = new()
                {
                    ProjectTitle = project.Name, Members = members, ProjectId = projectId, CreatedById = project.UserId
                };
                return View(teamMembersModel);
            }
            TempData["msg"] = "project not found!!";
            return RedirectToAction("Detail", new { projectId});
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThread(string projectId, string title)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Invalid model state";
                return RedirectToAction("Detail", new { projectId});
            }
            if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(title))
            {
                Console.Write("project id = " + projectId);
                Console.Write("title = " + title);
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var thread = await _projectRepository.CreateThread(user.Id, projectId, title);
                    if (thread != null)
                    {
                        TempData["msg"] = "New Message Thread created";
                        return RedirectToAction("Detail", new { projectId});
                    }
                    TempData["msg"] = "Failed to create New Message Thread";
                    return RedirectToAction("Detail", new { projectId});
                }
                TempData["msg"] = "User not found";
                return RedirectToAction("Detail", new { projectId});
            }
            TempData["msg"] = "Invalid model state";
            return RedirectToAction("Detail", new { projectId});
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(string threadId, string content, string projectId)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser != null)
                {
                    var result = await _projectRepository.AddMessageToThread(currentUser, threadId, content);
                    if (result)
                    {
                        TempData["msg"] = "Message sent successfully";
                        return RedirectToAction("Detail", new { projectId});
                    }
                    TempData["msg"] = "Failed to add message to thread";
                    return RedirectToAction("Detail", new { projectId});
                }
                TempData["msg"] = "User not found";
                return RedirectToAction("Detail", new { projectId});
            }
            TempData["msg"] = "invalid model state";
            return RedirectToAction("Detail", new { projectId});
        }
        // public async Task<JsonResult> CreateMessage(string threadId, string content)
        // {
        //     if (!string.IsNullOrEmpty(threadId) && !string.IsNullOrEmpty(content))
        //     {
        //         var currentUser = _userManager.GetUserAsync(User).Result;
        //         if (currentUser != null)
        //         {
        //             var result = await _projectRepository.AddMessageToThread(currentUser, threadId, content);
        //             if (result)
        //             {
        //                 return Json(new { success = true, message = "Message created successfully" });
        //             }
        //             return Json(new { error = true, message = "Failed to create message" });
        //         }
        //         return Json(new { error = true, message = "User is not logged in" });
        //     }
        //     return Json(new { error = true, message = "Validation error" });
        // }

    }    
} 