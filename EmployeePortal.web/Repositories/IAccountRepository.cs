using EmployeePortal.Models;
using System.Threading.Tasks;

namespace EmployeePortal.Repositories
{
    public interface IAccountRepository
    {
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameTakenAsync(string username);
        Task RegisterAsync(AppUser user);
        Task UpdateAsync(AppUser user);

    }
}
