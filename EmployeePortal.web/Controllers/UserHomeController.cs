using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeePortal.Repositories;

[Authorize(Roles = "User")]
public class UserHomeController : Controller
{
    private readonly IEmployeeRepository _employeeRepo;

    public UserHomeController(IEmployeeRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var employee = await _employeeRepo.GetByAppUserIdAsync(userId); // implement this if not available

        ViewBag.FullName = employee?.FullName ?? "User";
        return View();
    }
}
