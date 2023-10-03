using BasecampMVC.Models;

namespace BasecampMVC.Data
{
    public class ProjectTeamMembersModel
    {
        public string ProjectTitle { get; set; }
        public string CreatedById { get; set; }
        public string ProjectId { get; set; }
        public List<ApplicationUser> Members { get; set; }
    }
}