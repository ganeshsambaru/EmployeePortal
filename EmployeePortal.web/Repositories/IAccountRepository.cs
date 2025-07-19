using EmployeePortal.Models;
using EmployeePortal.ViewModels;
using System.Threading.Tasks;

namespace EmployeePortal.Repositories
{
    public interface IAccountRepository
    {
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameTakenAsync(string username);


        Task<AppUser> ValidateUserAsync(LoginViewModel vm);
        Task<AppUser> RegisterAsync(AppUser user);

        Task UpdateAsync(AppUser user);

        Task<AppUser> GetByUsernameOnlyAsync(string username);
        Task UpdateAdminAsync(AppUser user);


    }
}
