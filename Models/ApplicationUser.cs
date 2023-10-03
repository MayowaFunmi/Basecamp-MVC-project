using Microsoft.AspNetCore.Identity;

namespace BasecampMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation property for projects associated with the user
        public List<Project> Projects { get; set; }
        // Navigation property for teams (projects) the user is a member of
        // public List<Project> ProjectTeams { get; set; }
    }
}