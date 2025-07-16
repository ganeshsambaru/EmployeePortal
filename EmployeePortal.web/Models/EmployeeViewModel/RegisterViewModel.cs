using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }  // ✅ Required for Employee creation

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }  // Admin or User
    }
}
