using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class Payslip
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; }

        [Required]
        public string Month { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        public string Remarks { get; set; } = string.Empty;

        [NotMapped]
        public decimal GrossSalary => Salary + Bonus - Deductions;
        public decimal NetSalary => Salary + Bonus - Deductions;

    }
}
