using BasecampMVC.Data;
using BasecampMVC.Models;

namespace BasecampMVC.Interface
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<IEnumerable<Project>> GetProjectByUserId(string Id);
        Task<Project> GetProjectById(string Id);
        Task<bool> Create(AddProjectViewModel projectViewModel, ApplicationUser user);
        Task<bool> Update(EditProjectViewModel editProject);
        Task<bool> Delete(string Id);
        Task<bool> AddTeamMembers(string projectId, string userId);
        Task<bool> RemoveTeamMember(string projectId, string userId);
        Task<List<ApplicationUser>> ProjectTeamMembers(Project project);
        Task<MessageThread> CreateThread(string userId, string projectId, string title);
        bool IsUserInTeamMembers(Project project, ApplicationUser user);
        bool Save();
    }
}