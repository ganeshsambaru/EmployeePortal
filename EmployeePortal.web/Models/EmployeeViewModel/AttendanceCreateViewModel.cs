using System;
using System.ComponentModel.DataAnnotations;
using EmployeePortal.Models;
using System.Collections.Generic;

namespace EmployeePortal.ViewModels
{
    public class AttendanceCreateViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public List<Employee> Employees { get; set; } // For dropdown
    }
}
