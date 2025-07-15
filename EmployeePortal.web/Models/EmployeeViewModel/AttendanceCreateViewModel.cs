using EmployeePortal.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        
        public List<Employee> Employees { get; set; } = new List<Employee>();// For dropdown
    }
}
