using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        OnLeave
    }

    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }
    }
}
