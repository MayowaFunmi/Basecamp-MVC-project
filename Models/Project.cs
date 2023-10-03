using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasecampMVC.Models
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectImageUrl { get; set; }
        public List<MessageThread> MessageThreads { get; set; }
        public List<ApplicationUser> TeamMembers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}