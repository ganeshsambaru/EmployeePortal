using EmployeePortal.Models;
using System.Collections.Generic;

namespace EmployeePortal.ViewModels
{
    public class EmployeeListViewModel
    {
        public List<Employee> Employees { get; set; }

        // Filters
        public string SearchName { get; set; }
        public string Department { get; set; }
        public EmployeeType? EmployeeType { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } // ✅ Total employees after filtering



        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public List<string> AllDepartments { get; set; }
    }
}
