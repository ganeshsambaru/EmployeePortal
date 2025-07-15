using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Department { get; set; }
        public string Position { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

    }
}
