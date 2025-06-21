using System.Collections.Generic;
using System.Linq;
using MyApplication.Web.Models;

namespace MyApplication.Web.Services
{
    public class LeaveRequestService
    {
        private static List<LeaveRequest> _leaveRequests = new List<LeaveRequest>();
        private static int _nextId = 1;

        public List<LeaveRequest> GetAll() => _leaveRequests;
        public List<LeaveRequest> GetByUser(int userId) => _leaveRequests.Where(l => l.UserId == userId).ToList();
        public LeaveRequest? GetById(int id) => _leaveRequests.FirstOrDefault(l => l.Id == id);
        public void Add(LeaveRequest request)
        {
            request.Id = _nextId++;
            _leaveRequests.Add(request);
        }
        public void Approve(int id, bool isApproved)
        {
            var req = GetById(id);
            if (req != null)
                req.IsApproved = isApproved;
        }
    }
} 