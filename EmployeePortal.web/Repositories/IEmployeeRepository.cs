using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeePortal.Models;

namespace EmployeePortal.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task AddAsync(Employee employee);
        Task<Employee?> GetByIdAsync(int id);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task<(List<Employee> Employees, int TotalCount)> GetFilteredAsync(string search, string department, EmployeeType? type, int page, int pageSize);
        Task<List<string>> GetAllDepartmentsAsync();
        Task<Dictionary<string, int>> GetGenderStatsAsync();
        Task<Dictionary<string, int>> GetDepartmentStatsAsync();
        Task<Dictionary<string, int>> GetMonthlyHireStatsAsync();
        Task<Employee> GetByEmailAsync(string email);



    }
}
