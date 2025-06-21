using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Data;
using MyApplication.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MyApplication.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userName = context.HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null || !user.IsSuperUser)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
            base.OnActionExecuting(context);
        }

        // GET: /Admin/Users
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: /Admin/EditUser/5
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();
            if (!ModelState.IsValid) return View(updatedUser);
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.PhotoPath = updatedUser.PhotoPath;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.IsSuperUser = updatedUser.IsSuperUser;
            user.Password = updatedUser.Password;
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        // GET: /Admin/LeaveRequests
        public IActionResult LeaveRequests()
        {
            var leaveRequests = _context.LeaveRequests.Include(lr => lr.User).ToList();
            return View(leaveRequests);
        }

        // POST: /Admin/ApproveLeaveRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveLeaveRequest(int id, bool isApproved)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.IsApproved = isApproved;
            _context.SaveChanges();
            return RedirectToAction("LeaveRequests");
        }

        // POST: /Admin/UpdateLeaveStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLeaveStatus(int id, LeaveRequestStatus status)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.Status = status;
            leaveRequest.IsApproved = status == LeaveRequestStatus.Approved;
            _context.SaveChanges();
            return RedirectToAction("LeaveRequests");
        }


        // GET: /Admin/CreateUser
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Users");
        }
    }
} 