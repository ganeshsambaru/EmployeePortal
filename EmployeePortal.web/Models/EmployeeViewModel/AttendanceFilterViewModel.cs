using System;
using System.Collections.Generic;
using EmployeePortal.Models;

namespace EmployeePortal.ViewModels
{
    public class AttendanceFilterViewModel
    {
        public int? EmployeeId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<Employee> Employees { get; set; }

        public List<Attendance> Records { get; set; }
    }
}
