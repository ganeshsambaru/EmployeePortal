using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmployeePortal.ViewModels;
using EmployeePortal.Models;
using EmployeePortal.Repositories;
using Org.BouncyCastle.Crypto.Generators;

namespace EmployeePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepo;

        public AccountController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
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

            if (user == null || !BCrypt.Net.BCrypt.Verify(vm.Password, user.PasswordHash))
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

            return RedirectToAction("Dashboard", "Employees");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                Role = vm.Role
            };

            await _accountRepo.RegisterAsync(newUser);

            // 🧑 Also create an Employee linked to this user
            var employee = new Employee
            {
                FullName = vm.Username,
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

            var db = HttpContext.RequestServices.GetService<EmployeePortal.Data.EmployeeDbContext>();
            db.Employees.Add(employee);
            await db.SaveChangesAsync();

            TempData["Success"] = "Registration successful! Please login.";
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

            var username = User.Identity.Name;

            var user = await _accountRepo.GetByUsernameAsync(username);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(vm);
            }

            // 🔐 Verify current password
            if (!BCrypt.Net.BCrypt.Verify(vm.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                return View(vm);
            }

            // ✅ Hash and update new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            await _accountRepo.UpdateAsync(user);  // ⬅️ Make sure this method exists

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile", "Employees");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
