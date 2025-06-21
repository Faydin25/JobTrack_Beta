using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Models;
using MyApplication.Web.Services;
using System.Linq;

namespace MyApplication.Web.Controllers
{
    public class LeaveController : Controller
    {
        private static LeaveRequestService _service = new LeaveRequestService();

        // Kullanıcı izin başvuru formu
        public IActionResult Request()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Request(LeaveRequest model)
        {
            // Örnek: Giriş yapan kullanıcıyı 1 olarak varsayalım
            model.UserId = 1;
            _service.Add(model);
            return RedirectToAction("MyLeaves");
        }

        // Kullanıcı kendi izinlerini görsün
        public IActionResult MyLeaves()
        {
            var leaves = _service.GetByUser(1); // Örnek: Giriş yapan kullanıcı 1
            return View(leaves);
        }

        // Yönetici tüm izinleri görsün
        public IActionResult All()
        {
            var leaves = _service.GetAll();
            return View(leaves);
        }

        // Yönetici izin onaylasın
        [HttpPost]
        public IActionResult Approve(int id, bool approve)
        {
            _service.Approve(id, approve);
            return RedirectToAction("All");
        }
    }
} 