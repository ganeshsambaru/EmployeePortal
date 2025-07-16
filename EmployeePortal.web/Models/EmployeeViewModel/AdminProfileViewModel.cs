using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EmployeePortal.ViewModels
{
    public class AdminProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string? ProfileImagePath { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
