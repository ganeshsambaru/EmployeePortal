using EmployeePortal.Models;

namespace EmployeePortal.Repositories
{
    public interface IPayslipRepository
    {
        Task<List<Payslip>> GetAllAsync();
        Task<List<Payslip>> GetByEmployeeIdAsync(int employeeId);
        Task<Payslip?> GetByIdAsync(int id);
        Task AddAsync(Payslip payslip);
        Task UpdateAsync(Payslip payslip);
        Task DeleteAsync(int id);
    }
}
