using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EmployeePortal.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private readonly EmployeeDbContext _context;

        public LeaveRequestsController(EmployeeDbContext context)
        {
            _context = context;
        }

        // View all (Admin) or My leaves (Employee)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var leaves = await _context.LeaveRequests
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();

            return View(leaves);
        }

        // GET: Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var request = new LeaveRequest
            {
                EmployeeName = vm.EmployeeName,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate,
                Reason = vm.Reason,
                Status = LeaveStatus.Pending,
                CreatedDate = DateTime.Now
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Leave request submitted!";
            return RedirectToAction("Index");
        }

        // Approve
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = LeaveStatus.Approved;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Reject
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave != null)
            {
                leave.Status = LeaveStatus.Rejected;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
