using EmployeePortal.Data;
using EmployeePortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeePortal.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EmployeeDbContext _context;

        public AccountRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.Username == username);
        }

        public async Task RegisterAsync(AppUser user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AppUser user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
