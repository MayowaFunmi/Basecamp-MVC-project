using System.ComponentModel.DataAnnotations;

namespace BasecampMVC.Data
{
    public class AddProjectViewModel
    {
        [Required]
        [Display(Name = "Project Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }

        [Display(Name = "Project Image")]
        public IFormFile ImageFile { get; set; }
        public string? Url { get; set; }
    }
}