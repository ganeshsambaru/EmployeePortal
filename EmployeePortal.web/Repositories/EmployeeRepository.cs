using EmployeePortal.Data;
using EmployeePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _context;

        public EmployeeRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetFilteredAsync(string search, string department, EmployeeType? type, int page, int pageSize)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.FullName.Contains(search));

            if (!string.IsNullOrEmpty(department))
                query = query.Where(e => e.Department == department);

            if (type.HasValue)
                query = query.Where(e => e.EmployeeType == type.Value);

            var total = await query.CountAsync();

            var employees = await query
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, total);
        }

        public async Task<List<string>> GetAllDepartmentsAsync()
        {
            return await _context.Employees
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetGenderStatsAsync()
        {
            var employees = await _context.Employees.ToListAsync(); // Load all data in memory

            var result = employees
                .GroupBy(e => e.Gender.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            return result;
        }


        public async Task<Dictionary<string, int>> GetDepartmentStatsAsync()
        {
            return await _context.Employees
                .GroupBy(e => e.Department)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }


        public async Task<Dictionary<string, int>> GetMonthlyHireStatsAsync()
        {
            var now = DateTime.Now;
            var sixMonthsAgo = now.AddMonths(-5);

            // Pull employees first from database
            var employees = await _context.Employees
                .Where(e => e.HireDate >= sixMonthsAgo)
                .ToListAsync(); // DB call ends here

            // Group and project in memory
            var hires = employees
                .GroupBy(e => e.HireDate.ToString("MMM yyyy"))
                .ToDictionary(g => g.Key, g => g.Count());

            // Fill missing months
            var result = new Dictionary<string, int>();
            for (int i = 0; i < 6; i++)
            {
                var month = now.AddMonths(-i).ToString("MMM yyyy");
                result[month] = hires.ContainsKey(month) ? hires[month] : 0;
            }

            return result.Reverse().ToDictionary(x => x.Key, x => x.Value);
        }


        public async Task<Employee> GetByEmailAsync(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Employee?> GetByAppUserNameAsync(string username)
        {
            return await _context.Employees
                .Include(e => e.AppUser)
                .FirstOrDefaultAsync(e => e.AppUser.Username == username);
        }
    }
}
