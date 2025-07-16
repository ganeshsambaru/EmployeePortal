using System;
using System.ComponentModel.DataAnnotations;
using EmployeePortal.Models;

namespace EmployeePortal.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Position { get; set; }
        public string Department { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public EmployeeType EmployeeType { get; set; }

        public decimal Salary { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }

    }
}
