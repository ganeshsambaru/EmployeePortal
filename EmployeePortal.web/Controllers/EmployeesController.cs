using EmployeePortal.Models;
using EmployeePortal.Repositories;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;


namespace EmployeePortal.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAccountRepository _accountRepo;

        public EmployeesController(IEmployeeRepository repo, IAccountRepository accountRepo)
        {
            _repo = repo;
            _accountRepo = accountRepo;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchName, string department, EmployeeType? employeeType, int page = 1)
        {
            const int pageSize = 5;

            var (employees, totalCount) = await _repo.GetFilteredAsync(searchName, department, employeeType, page, pageSize);

            var allDepartments = await _repo.GetAllDepartmentsAsync();

            var vm = new EmployeeListViewModel
            {
                Employees = employees,
                SearchName = searchName,
                Department = department,
                EmployeeType = employeeType,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                AllDepartments = allDepartments
            };

            return View(vm);
        }


        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            string? imagePath = null;
            if (vm.ProfileImage != null && vm.ProfileImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ProfileImage.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.ProfileImage.CopyToAsync(stream);
                imagePath = "/uploads/" + fileName;
            }

            var employee = new Employee
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Position = vm.Position,
                Department = vm.Department,
                HireDate = vm.HireDate,
                DateOfBirth = vm.DateOfBirth,
                Gender = vm.Gender,
                EmployeeType = vm.EmployeeType,
                Salary = vm.Salary,
                ProfileImagePath = imagePath
            };

            await _repo.AddAsync(employee);
            TempData["Success"] = "Employee created successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditAdminProfile()
        {
            var username = User.Identity?.Name;
            var user = await _accountRepo.GetByUsernameAsync(username);

            if (user == null)
            {
                TempData["Error"] = "Admin profile not found.";
                return RedirectToAction("Dashboard", "Employees");
            }

            var vm = new AdminProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                ProfileImagePath = user.ProfileImagePath
            };

            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(AdminProfileViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _accountRepo.GetByUsernameAsync(User.Identity.Name);
            if (user == null) return NotFound();

            user.FullName = vm.FullName;

            if (vm.ProfileImage != null && vm.ProfileImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ProfileImage.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImagePath = "/uploads/" + fileName;
            }

            await _accountRepo.UpdateAsync(user);

            TempData["Success"] = "Admin profile updated successfully!";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            var vm = new EmployeeDetailsViewModel
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Email = emp.Email,
                Position = emp.Position,
                Department = emp.Department,
                HireDate = emp.HireDate,
                DateOfBirth = emp.DateOfBirth,
                Gender = emp.Gender,
                EmployeeType = emp.EmployeeType,
                Salary = emp.Salary
            };

            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            return View(emp); // You can use the Employee model directly for confirmation
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            TempData["Success"] = "Employee updated successfully!";
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportPdf()
        {
            var employees = await _repo.GetAllAsync();

            return new ViewAsPdf("PdfTemplate", employees)
            {
                FileName = "EmployeeList.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportExcel()
        {
            var employees = await _repo.GetAllAsync();

            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Employees");

            // Header row
            worksheet.Cells[1, 1].Value = "Full Name";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Department";
            worksheet.Cells[1, 4].Value = "Position";
            worksheet.Cells[1, 5].Value = "Employee Type";
            worksheet.Cells[1, 6].Value = "Salary";

            // Data rows
            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cells[row, 1].Value = emp.FullName;
                worksheet.Cells[row, 2].Value = emp.Email;
                worksheet.Cells[row, 3].Value = emp.Department;
                worksheet.Cells[row, 4].Value = emp.Position;
                worksheet.Cells[row, 5].Value = emp.EmployeeType.ToString();
                worksheet.Cells[row, 6].Value = emp.Salary;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmployeeList.xlsx"
            );
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGenderChartData()
        {
            var stats = await _repo.GetGenderStatsAsync();

            var result = new
            {
                labels = stats.Keys,
                counts = stats.Values
            };

            return Json(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDepartmentChartData()
        {
            var stats = await _repo.GetDepartmentStatsAsync();

            var result = new
            {
                labels = stats.Keys,
                counts = stats.Values
            };

            return Json(result);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHireChartData()
        {
            var stats = await _repo.GetMonthlyHireStatsAsync();

            var result = new
            {
                labels = stats.Keys,
                counts = stats.Values
            };

            return Json(result);
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;

            if (User.IsInRole("Admin"))
            {
                var admin = await _accountRepo.GetByUsernameOnlyAsync(username);
                if (admin == null) return NotFound();

                var vm = new AdminProfileViewModel
                {
                    Id = admin.Id,
                    Username = admin.Username,
                    Role = admin.Role,
                    FullName = admin.FullName,
                    ProfileImagePath = admin.ProfileImagePath
                };

                return View("AdminProfile", vm);
            }

            // Regular employee
            var employee = await _repo.GetByAppUserNameAsync(username);
            if (employee == null)
            {
                TempData["Error"] = "Employee profile not found.";
                return RedirectToAction("Dashboard");
            }

            return View(employee);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            var vm = new EmployeeEditViewModel
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Email = emp.Email,
                Position = emp.Position,
                Department = emp.Department,
                HireDate = emp.HireDate,
                DateOfBirth = emp.DateOfBirth,
                Gender = emp.Gender,
                EmployeeType = emp.EmployeeType,
                Salary = emp.Salary,
                ProfileImagePath = emp.ProfileImagePath
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var emp = await _repo.GetByIdAsync(id);
            if (emp == null) return NotFound();

            emp.FullName = vm.FullName;
            emp.Email = vm.Email;
            emp.Position = vm.Position;
            emp.Department = vm.Department;
            emp.HireDate = vm.HireDate;
            emp.DateOfBirth = vm.DateOfBirth;
            emp.Gender = vm.Gender;
            emp.EmployeeType = vm.EmployeeType;
            emp.Salary = vm.Salary;

            if (vm.ProfileImage != null && vm.ProfileImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ProfileImage.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.ProfileImage.CopyToAsync(stream);
                emp.ProfileImagePath = "/uploads/" + fileName;
            }

            await _repo.UpdateAsync(emp);
            TempData["Success"] = "Employee updated successfully!";
            return RedirectToAction("Index");
        }


    }
}


