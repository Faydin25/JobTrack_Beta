using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Web.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public int LeaveDaysUsed { get; set; } = 0;
        public string LeaveDatesJson { get; set; } = "[]";
        [NotMapped]
        public List<DateTime> LeaveDates
        {
            get => string.IsNullOrEmpty(LeaveDatesJson) ? new List<DateTime>() : System.Text.Json.JsonSerializer.Deserialize<List<DateTime>>(LeaveDatesJson);
            set => LeaveDatesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        [NotMapped]
        public int TotalDaysWorked
        {
            get
            {
                if (StartDate == null) return 0;
                return (DateTime.Now.Date - StartDate.Value.Date).Days + 1;
            }
        }
        [NotMapped]
        public int FailedTaskCount => Tasks?.Count(t => t.Status != TaskStatus.Done && t.Deadline < DateTime.Now) ?? 0;
        [NotMapped]
        public int CompletedTaskCount => Tasks?.Count(t => t.Status == TaskStatus.Done) ?? 0;

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public string LogTimesJson { get; set; } = "[]";
        public bool IsSuperUser { get; set; } = false;
    }
}
