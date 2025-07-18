using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmployeePortal.ViewModels;
using EmployeePortal.Models;
using EmployeePortal.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepo;
        private readonly EmployeePortal.Data.EmployeeDbContext _context;

        public AccountController(IAccountRepository accountRepo, EmployeePortal.Data.EmployeeDbContext context)
        {
            _accountRepo = accountRepo;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _accountRepo.GetByUsernameAsync(vm.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(vm.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(vm);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            
            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Employees");
            }
            else
            {
                return RedirectToAction("Index", "UserHome");
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (await _accountRepo.IsUsernameTakenAsync(vm.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(vm);
            }

            var newUser = new AppUser
            {
                Username = vm.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                Role = vm.Role
            };

            newUser = await _accountRepo.RegisterAsync(newUser);
            Console.WriteLine($"Registered role: {vm.Role}");

            // Optional: Only create Employee if Role is "User"
            if (newUser.Role == "User")
            {
                var employee = new Employee
                {
                    FullName = vm.FullName ?? vm.Username,
                    Email = vm.Username,
                    HireDate = DateTime.Now,
                    Department = "Not Assigned",
                    Position = "Not Assigned",
                    EmployeeType = EmployeeType.Permanent,
                    Salary = 0,
                    Gender = Gender.Other,
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    AppUserId = newUser.Id

                };

                _context.Employees.Add(employee);
                Console.WriteLine($"Registered role: {vm.Role}");

                await _context.SaveChangesAsync();
            }


           // TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var username = User.Identity?.Name;
            var user = await _accountRepo.GetByUsernameAsync(username);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(vm);
            }

            if (!BCrypt.Net.BCrypt.Verify(vm.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                return View(vm);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            await _accountRepo.UpdateAsync(user);

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile", "Employees");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
