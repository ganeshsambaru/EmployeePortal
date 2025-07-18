using EmployeePortal.Models;
using EmployeePortal.Repositories;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace EmployeePortal.Controllers
{
    [Authorize]
    public class PayslipsController : Controller
    {
        private readonly IPayslipRepository _payslipRepo;
        private readonly IEmployeeRepository _employeeRepo;

        public PayslipsController(IPayslipRepository payslipRepo, IEmployeeRepository employeeRepo)
        {
            _payslipRepo = payslipRepo;
            _employeeRepo = employeeRepo;
        }

        // 👨‍💼 Admin: List all payslips
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var payslips = await _payslipRepo.GetAllAsync();
            return View(payslips);
        }

        // 👷‍♂️ User: View only their payslips
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyPayslips()
        {
            var username = User.Identity?.Name;
            var employee = await _employeeRepo.GetByAppUserNameAsync(username);

            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction("Dashboard", "Employees");
            }

            var payslips = await _payslipRepo.GetByEmployeeIdAsync(employee.Id);
            return View(payslips);
        }

        // 👨‍💼 Admin: Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var employees = await _employeeRepo.GetAllAsync();

            var viewModel = new PayslipViewModel
            {
                EmployeeList = employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                })
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(PayslipViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeRepo.GetAllAsync();
                return View(vm);
            }

            var payslip = new Payslip
            {
                EmployeeId = vm.EmployeeId,
                Salary = vm.Salary,
                Bonus = vm.Bonus,
                Deductions = vm.Deductions,
                Month = vm.Month,
                Year = vm.Year,
                Remarks = vm.Remarks
            };

            await _payslipRepo.AddAsync(payslip);
            TempData["Success"] = "Payslip created successfully!";
            return RedirectToAction("Index");
        }

        // 👨‍💼 Admin: Edit
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var payslip = await _payslipRepo.GetByIdAsync(id);
            if (payslip == null) return NotFound();

            var employees = await _employeeRepo.GetAllAsync();

            var viewModel = new PayslipViewModel
            {
                Id = payslip.Id,
                EmployeeId = payslip.EmployeeId,
                Salary = payslip.Salary,
                Bonus = payslip.Bonus,
                Deductions = payslip.Deductions,
                Month = payslip.Month,
                Year = payslip.Year,
                Remarks = payslip.Remarks,
                EmployeeList = employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                })
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, PayslipViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeRepo.GetAllAsync();
                return View(vm);
            }

            var employee = await _employeeRepo.GetByIdAsync(vm.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("EmployeeId", "Selected employee does not exist.");
                ViewBag.Employees = await _employeeRepo.GetAllAsync();
                return View(vm);
            }

            var payslip = await _payslipRepo.GetByIdAsync(id);
            if (payslip == null) return NotFound();

            payslip.EmployeeId = vm.EmployeeId;
            payslip.Salary = vm.Salary;
            payslip.Bonus = vm.Bonus;
            payslip.Deductions = vm.Deductions;
            payslip.Month = vm.Month;
            payslip.Year = vm.Year;
            payslip.Remarks = vm.Remarks;

            await _payslipRepo.UpdateAsync(payslip);
            TempData["Success"] = "Payslip updated successfully!";
            return RedirectToAction("Index");
        }

        // 👨‍💼 Admin: Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var payslip = await _payslipRepo.GetByIdAsync(id);
            if (payslip == null) return NotFound();

            await _payslipRepo.DeleteAsync(id);
            TempData["Success"] = "Payslip deleted successfully!";
            return RedirectToAction("Index");
        }

       

public async Task<IActionResult> DownloadPdf(int id)
    {
        var payslip = await _payslipRepo.GetByIdAsync(id);
        if (payslip == null)
            return NotFound();

        // Include Employee details (if not already loaded)
        payslip.Employee = await _employeeRepo.GetByIdAsync(payslip.EmployeeId);

        return new ViewAsPdf("PayslipPdf", payslip)
        {
            FileName = $"Payslip_{payslip.Employee.FullName}_{payslip.Month}_{payslip.Year}.pdf"
        };
    }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var payslip = await _payslipRepo.GetByIdAsync(id);
            if (payslip == null)
                return NotFound();

            var vm = new PayslipViewModel
            {
                Id = payslip.Id,
                EmployeeId = payslip.EmployeeId,
                Salary = payslip.Salary,
                Bonus = payslip.Bonus,
                Deductions = payslip.Deductions,
                Month = payslip.Month,
                Year = payslip.Year,
                Remarks = payslip.Remarks,
                // Include more fields if needed
            };

            return View(vm);
        }


    }
}
