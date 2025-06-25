using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Data;
using MyApplication.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyApplication.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            user.StartDate = updatedUser.StartDate;
            user.IsSuperUser = updatedUser.IsSuperUser;
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = updatedUser.Password;
            }
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        // GET: /Admin/NewsList
        public IActionResult NewsList()
        {
            var news = _context.News.OrderByDescending(n => n.PublishedDate).ToList();
            return View(news);
        }

        // GET: /Admin/CreateNews
        public IActionResult CreateNews()
        {
            return View();
        }

        // POST: /Admin/CreateNews
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(News news)
        {
            if (ModelState.IsValid)
            {
                var imageFile = Request.Form.Files["imageFile"];
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/news");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await imageFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    news.ImagePath = "/images/news/" + uniqueFileName;
                }
                news.PublishedDate = DateTime.Now;
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(NewsList));
            }
            return View(news);
        }

        // GET: /Admin/EditNews/5
        public async Task<IActionResult> EditNews(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        // POST: /Admin/EditNews/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(int id, News news, IFormFile imageFile)
        {
            if (id != news.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/news");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await imageFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    news.ImagePath = "/images/news/" + uniqueFileName;
                }

                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.News.Any(e => e.Id == news.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(NewsList));
            }
            return View(news);
        }

        // GET: /Admin/DeleteNews/5
        public async Task<IActionResult> DeleteNews(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null) return NotFound();
            return View(news);
        }

        // POST: /Admin/DeleteNews/5
        [HttpPost, ActionName("DeleteNews")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NewsList));
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

        // GET: /Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUserConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
    }
} 