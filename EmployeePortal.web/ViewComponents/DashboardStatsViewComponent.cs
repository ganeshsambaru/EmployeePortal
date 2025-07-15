using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeePortal.ViewComponents
{
    public class DashboardStatsViewComponent : ViewComponent
    {
        private readonly EmployeeDbContext _context;

        public DashboardStatsViewComponent(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var total = await _context.Employees.CountAsync();
            var avgSalary = await _context.Employees.AverageAsync(e => e.Salary);
            var male = await _context.Employees.CountAsync(e => e.Gender == Gender.Male);
            var female = await _context.Employees.CountAsync(e => e.Gender == Gender.Female);
            var other = await _context.Employees.CountAsync(e => e.Gender == Gender.Other);

            var stats = new DashboardStatsViewModel
            {
                TotalEmployees = total,
                AverageSalary = avgSalary,
                MaleCount = male,
                FemaleCount = female,
                OtherCount = other
            };

            return View(stats);
        }
    }
}
