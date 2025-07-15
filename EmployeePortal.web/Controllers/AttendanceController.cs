using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Rotativa.AspNetCore;

namespace EmployeePortal.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly EmployeeDbContext _context;

        public AttendanceController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: Attendance/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = new AttendanceCreateViewModel
            {
                Date = DateTime.Today,
                Employees = await _context.Employees.ToListAsync()
            };

            return View(vm);
        }

        // POST: Attendance/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Employees = await _context.Employees.ToListAsync();
                return View(vm);
            }

            // Optional: prevent duplicate entry for same employee & date
            bool alreadyMarked = await _context.Attendances
                .AnyAsync(a => a.EmployeeId == vm.EmployeeId && a.Date == vm.Date);

            if (alreadyMarked)
            {
                ModelState.AddModelError("", "Attendance already marked for this employee and date.");
                vm.Employees = await _context.Employees.ToListAsync();
                return View(vm);
            }

            var attendance = new Attendance
            {
                EmployeeId = vm.EmployeeId,
                Date = vm.Date,
                Status = vm.Status
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Attendance marked successfully!";
            return RedirectToAction("Records");
        }

        // GET: Attendance/Records
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Records(int? employeeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId);

            if (fromDate.HasValue)
                query = query.Where(a => a.Date >= fromDate);

            if (toDate.HasValue)
                query = query.Where(a => a.Date <= toDate);

            var vm = new AttendanceFilterViewModel
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                Employees = await _context.Employees.ToListAsync(),
                Records = await query.OrderByDescending(a => a.Date).ToListAsync()
            };

            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportPdf()
        {
            var data = await _context.Attendances.Include(a => a.Employee).ToListAsync();

            return new ViewAsPdf("AttendancePdfTemplate", data)
            {
                FileName = "AttendanceReport.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportExcel()
        {
            var data = await _context.Attendances.Include(a => a.Employee).ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Attendance");

            sheet.Cells[1, 1].Value = "Date";
            sheet.Cells[1, 2].Value = "Employee";
            sheet.Cells[1, 3].Value = "Status";

            int row = 2;
            foreach (var a in data)
            {
                sheet.Cells[row, 1].Value = a.Date.ToShortDateString();
                sheet.Cells[row, 2].Value = a.Employee.FullName;
                sheet.Cells[row, 3].Value = a.Status.ToString();
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceReport.xlsx");
        }

        [Authorize(Roles = "User")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyAttendance()
        {
            var username = User.Identity?.Name;

            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Dashboard", "Employees");
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.AppUserId == user.Id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not linked.";
                return RedirectToAction("Dashboard", "Employees");
            }

            var records = await _context.Attendances
                .Where(a => a.EmployeeId == employee.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(records);
        }



    }
}