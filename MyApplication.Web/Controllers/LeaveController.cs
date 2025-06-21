using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Data;
using MyApplication.Web.Models;
using System.Linq;
using System.Security.Claims;

namespace MyApplication.Web.Controllers
{
    public class LeaveController : Controller
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }

        // Kullanıcı izin başvuru formu
        public IActionResult Request()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Request(LeaveRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            model.UserId = int.Parse(userId);
            _context.LeaveRequests.Add(model);
            _context.SaveChanges();
            return RedirectToAction("MyLeaves");
        }

        // Kullanıcı kendi izinlerini görsün
        public IActionResult MyLeaves()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userIdInt = int.Parse(userId);
            var leaves = _context.LeaveRequests.Where(l => l.UserId == userIdInt).ToList();
            return View(leaves);
        }

        // Yönetici tüm izinleri görsün
        public IActionResult All()
        {
            var leaves = _context.LeaveRequests.ToList();
            return View(leaves);
        }

        // Yönetici izin onaylasın
        [HttpPost]
        public IActionResult Approve(int id, bool approve)
        {
            var req = _context.LeaveRequests.Find(id);
            if (req != null)
            {
                req.IsApproved = approve;
                _context.SaveChanges();
            }
            return RedirectToAction("All");
        }
    }
} 