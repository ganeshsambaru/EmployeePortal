using System;
using EmployeePortal.Models;

namespace EmployeePortal.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public decimal Salary { get; set; }
    }
}
