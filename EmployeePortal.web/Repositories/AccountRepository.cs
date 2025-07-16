using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EmployeeDbContext _context;

        public AccountRepository(EmployeeDbContext context)
        {
            _context = context;
        }
        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task UpdateAsync(AppUser user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }

       

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.Username == username);
        }


        public async Task<AppUser> RegisterAsync(AppUser user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync(); // <== This MUST exist
            return user;
        }



        public async Task<AppUser> ValidateUserAsync(LoginViewModel vm)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == vm.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(vm.Password, user.Password))
                return user;

            return null;
        }
    }
}
