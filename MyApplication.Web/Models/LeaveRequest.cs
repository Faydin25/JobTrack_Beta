using System;
using System.ComponentModel.DataAnnotations;

namespace MyApplication.Web.Models
{
    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

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
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
        public virtual User? User { get; set; }
    }
} 