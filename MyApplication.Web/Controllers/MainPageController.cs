using Microsoft.AspNetCore.Mvc;
using MyApplication.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyApplication.Web.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MyApplication.Web.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        private readonly AppDbContext _context;
        public MainPageController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var currentUser = _context.Users
                    .Include(u => u.Tasks)
                    .FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var users = _context.Users.Include(u => u.Tasks).ToList();
                var ageGroups = GroupsAge(users);
                var userCount = users.Count;
                var tasksAndCount = GetCountTasks(users);

                // Gecikmiş taskleri hesapla
                var overdueTasksCount = _context.Tasks.Count(t => t.Deadline < DateTime.Now && t.Status != MyApplication.Web.Models.TaskStatus.Done);

                var usersWithPhoto = users.Count(u => !string.IsNullOrEmpty(u.PhotoPath));
                var usersWithoutPhoto = users.Count(u => string.IsNullOrEmpty(u.PhotoPath));

                var news = _context.News.OrderByDescending(n => n.PublishedDate).Take(10).ToList();

                ViewData["UsersWithPhoto"] = usersWithPhoto;
                ViewData["UsersWithoutPhoto"] = usersWithoutPhoto;
                ViewData["AgeGroups"] = ageGroups;
                ViewData["UserCount"] = userCount;
                ViewData["TasksAndCount"] = tasksAndCount;
                ViewData["UserLogs"] = currentUser.LogTimesJson ?? "[]";
                ViewData["OverdueTasksCount"] = overdueTasksCount;
                ViewData["News"] = news;

                return View();
            }
            catch (Exception ex)
            {
                // Log the error if you have logging configured
                return RedirectToAction("Index", "Home");
            }
        }

        private int CalculateAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                return 0;
            }
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Value.Year;

            if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        private Dictionary<int, int> GroupsAge(List<User> users)
        {
            var ageGroups = new Dictionary<int, int>();
            foreach (var user in users)
            {
                var age = CalculateAge(user.DateOfBirth);
                if (ageGroups.ContainsKey(age)) 
                    ageGroups[age]++;
                else
                    ageGroups[age] = 1;
            }
            return ageGroups;
        }

        private Dictionary<string, int> GetCountTasks(List<User> users)
        {
            var countTasks = new Dictionary<string, int>
            {
                { "ToDo", 0 },
                { "InProgress", 0 },
                { "Done", 0 }
            };

            foreach (var user in users)
            {
                if (user.Tasks != null)
                {
                    foreach (var task in user.Tasks)
                    {
                        switch (task.Status)
                        {
                            case Models.TaskStatus.ToDo:
                                countTasks["ToDo"]++;
                                break;
                            case Models.TaskStatus.InProgress:
                                countTasks["InProgress"]++;
                                break;
                            case Models.TaskStatus.Done:
                                countTasks["Done"]++;
                                break;
                        }
                    }
                }
            }

            return countTasks;
        }
    }
}
