using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Data;
using MyApplication.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using PhoneNumbers;

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
            if (!ModelState.IsValid)
            {
                // Eski şifreyi view modeline tekrar koy
                var existingUser = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
                if (existingUser != null)
                    updatedUser.Password = existingUser.Password;
                return View(updatedUser);
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.PhotoPath = updatedUser.PhotoPath;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.IsSuperUser = updatedUser.IsSuperUser;
            // Telefon numarası doğrulama
            if (!string.IsNullOrEmpty(updatedUser.PhoneNumber))
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                try
                {
                    var phoneNumber = phoneUtil.Parse(updatedUser.PhoneNumber, "TR");
                    if (phoneUtil.IsValidNumberForRegion(phoneNumber, "TR"))
                    {
                        user.PhoneNumber = updatedUser.PhoneNumber;
                    }
                    else
                    {
                        ModelState.AddModelError("PhoneNumber", "Geçerli bir telefon numarası giriniz.");
                        return View(updatedUser);
                    }
                }
                catch (NumberParseException)
                {
                    ModelState.AddModelError("PhoneNumber", "Geçerli bir telefon numarası giriniz.");
                    return View(updatedUser);
                }
            }
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = updatedUser.Password;
            }
            _context.SaveChanges();
            return RedirectToAction("Users");
        }
    }
} 