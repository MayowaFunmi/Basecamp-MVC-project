using BasecampMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BasecampMVC.Data
{
    public class UserViewModelData
    {
        public ApplicationUser? User { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IList<string>? UserRoles { get; set; }
        public List<IdentityRole>? AvailableRoles { get; set; }
        public string SelectedRole { get; set; }
    }
}