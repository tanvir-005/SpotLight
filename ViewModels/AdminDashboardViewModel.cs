using System.Collections.Generic;
using SpotLight.Models;

namespace SpotLight.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int ActiveEvents { get; set; }
        public int PendingEventRequests { get; set; }
        public int TotalUsers { get; set; }
        public List<EventRequest> RecentEventRequests { get; set; }
        public List<Event> UpcomingEvents { get; set; }
    }
}