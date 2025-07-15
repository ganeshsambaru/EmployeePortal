using EmployeePortal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class EmployeeCreateViewModel
    {
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
        [Precision(18, 2)]
        public decimal Salary { get; set; }
    }
}
