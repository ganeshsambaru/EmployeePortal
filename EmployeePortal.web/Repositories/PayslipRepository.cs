using EmployeePortal.Data;
using EmployeePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Repositories
{
    public class PayslipRepository : IPayslipRepository
    {
        private readonly EmployeeDbContext _context;

        public PayslipRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payslip>> GetAllAsync()
        {
            return await _context.Payslips
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();
        }

        public async Task<List<Payslip>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Payslips
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();
        }

        public async Task<Payslip?> GetByIdAsync(int id)
        {
            return await _context.Payslips
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Payslip payslip)
        {
            _context.Payslips.Add(payslip);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payslip payslip)
        {
            _context.Payslips.Update(payslip);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip != null)
            {
                _context.Payslips.Remove(payslip);
                await _context.SaveChangesAsync();
            }
        }
    }
}
