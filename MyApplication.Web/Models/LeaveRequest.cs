using System;
using System.ComponentModel.DataAnnotations;

namespace MyApplication.Web.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? Description { get; set; }
        public bool IsApproved { get; set; } = false;
    }
} 