using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.ViewModels
{
    public class PayslipViewModel
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deductions { get; set; }

        [Required]
        public string Month { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        public string Remarks { get; set; } = string.Empty;

        // ✅ Calculated properties
        public decimal GrossSalary => Salary + Bonus;
        public decimal NetSalary => GrossSalary - Deductions;

        public IEnumerable<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
    }
}
