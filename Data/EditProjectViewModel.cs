using System.ComponentModel.DataAnnotations;

namespace BasecampMVC.Data
{
    public class EditProjectViewModel
    {
        [Required]
        public string ProjectId { get; set; }
        [Required]
        [Display(Name = "Project Title")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }
    }
}