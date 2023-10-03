using BasecampMVC.Data;
using BasecampMVC.Interface;
using BasecampMVC.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasecampMVC.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProjectRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<bool> Create(AddProjectViewModel projectViewModel, ApplicationUser user)
        {
            Project newProject = new()
            {
                UserId = user.Id,
                User = user,
                Name = projectViewModel.Name,
                Description = projectViewModel.Description,
                ProjectImageUrl = projectViewModel.Url!,
                CreatedAt = DateTime.Now
            };
            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();
            await AddTeamMembers(newProject.ProjectId.ToString(), user.Id);
            return true;
        }

        public async Task<bool> AddTeamMembers(string projectId, string userId)
        {
            var project = await GetProjectById(projectId);
            var user = await _userManager.FindByIdAsync(userId);
            if (project != null && user != null)
            {
                project.TeamMembers.Add(user);
                return Save();
            }
            return false;
        }

        public async Task<bool> RemoveTeamMember(string projectId, string userId)
        {
            var project = await GetProjectById(projectId);
            var user = await _userManager.FindByIdAsync(userId);
            if (project != null && user != null)
            {
                project.TeamMembers.Remove(user);
                return Save();
            }
            return false;
        }

        public async Task<List<ApplicationUser>> ProjectTeamMembers(Project project)
        {
            if (project != null)
            {
                return project.TeamMembers.ToList();
            }
            return new List<ApplicationUser>();
        }

        public async Task<bool> Delete(string Id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId.ToString() == Id);
            if (project != null)
            {
                var result = _context.Projects.Remove(project);
                return Save();
            }
            return false;
        }

        public Task<IEnumerable<Project>> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectById(string Id)
        {
            var project = await _context.Projects
                .Include(p => p.User)
                .Include(p => p.TeamMembers)
                .Include(p => p.MessageThreads)
                .ThenInclude(mt => mt.Messages)
                .FirstOrDefaultAsync(p => p.ProjectId.ToString() == Id);

            return project ?? default!;
        }

        public async Task<IEnumerable<Project>> GetProjectByUserId(string Id)
        {
            var userProjects = await _context.Projects.Include(p => p.User).Where(p => p.User.Id == Id).ToListAsync();
            return userProjects ?? default!;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public async Task<bool> Update(EditProjectViewModel editProject)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId.ToString() == editProject.ProjectId);
            if (project != null)
            {
                project.Name = editProject.Name;
                project.Description = editProject.Description;
                _context.Projects.Update(project);
                return Save();
            }
            return false;
        }

        public bool IsUserInTeamMembers(Project project, ApplicationUser user)
        {
            if (project != null && user != null)
            {
                return project.TeamMembers.Any(tm => tm.Id == user.Id);
            }
            return false;
        }

        public async Task<MessageThread> CreateThread(string userId, string projectId, string title)
        {
            var project = await GetProjectById(projectId);
            var user = await _userManager.FindByIdAsync(userId);
            if (project != null && user != null)
            {
                bool userIsInTeam = IsUserInTeamMembers(project, user);
                if (userIsInTeam)
                {
                    MessageThread messageThread = new()
                    {
                        ProjectId = project.ProjectId,
                        Project = project,
                        Title = title,
                        CreatedAt = DateTime.Now
                    };
                    var result = await _context.MessageThreads.AddAsync(messageThread);
                    project.MessageThreads.Add(messageThread);
                    await _context.SaveChangesAsync();
                    return result.Entity;
                }
                return null;
            }
            return null;
        }
    }
}