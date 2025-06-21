// BusinessPageViewModel.cs
namespace MyApplication.Web.Models
{
    public class BusinessPageViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public IEnumerable<MyApplication.Web.Models.Task> Tasks { get; set; } = new List<MyApplication.Web.Models.Task>();
        public TaskStatus? FilterStatus { get; set; }
        public int? FilterUserId { get; set; }
    }
}
