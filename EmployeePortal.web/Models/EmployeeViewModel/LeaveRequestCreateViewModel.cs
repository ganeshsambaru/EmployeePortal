using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class LeaveRequestCreateViewModel
    {
        

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}
