using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotLight.Models;
using SpotLight.Data;
using SpotLight.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SpotLight.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("AdminLogin", "Account");

            var viewModel = new AdminDashboardViewModel
            {
                TotalEvents = await _context.Events.CountAsync(),
                ActiveEvents = await _context.Events.CountAsync(e => e.Date >= System.DateTime.Today),
                PendingEventRequests = await _context.EventRequests.CountAsync(r => !r.Status),
                TotalUsers = await _context.Organizers.CountAsync() + await _context.Participants.CountAsync(),
                RecentEventRequests = await _context.EventRequests
                    .Include(r => r.Organizer)
                    .Where(r => !r.Status)
                    .OrderByDescending(r => r.RequestId)
                    .Take(5)
                    .ToListAsync(),
                UpcomingEvents = await _context.Events
                    .Include(e => e.Organizer)
                    .Where(e => e.Date >= System.DateTime.Today)
                    .OrderBy(e => e.Date)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // View all event requests
        public async Task<IActionResult> EventRequests()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("AdminLogin", "Account");

            var requests = await _context.EventRequests
                .Include(r => r.Organizer)
                .Where(r => !r.Status)
                .ToListAsync();
            return View(requests);
        }

        // Approve event request
        [HttpPost]
        public async Task<IActionResult> ApproveEventRequest(int id)
        {
            var request = await _context.EventRequests
                .Include(r => r.Organizer)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request != null)
            {
                var newEvent = new Event
                {
                    Title = request.Title,
                    Description = request.Description,
                    Date = request.Date,
                    Time = request.Time,
                    Venue = request.Venue,
                    TotalSeats = request.NumberOfSeats,
                    OrganizerId = request.OrganizerId
                };

                _context.Events.Add(newEvent);
                request.Status = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("EventRequests");
        }

        // Reject event request
        [HttpPost]
        public async Task<IActionResult> RejectEventRequest(int id)
        {
            var request = await _context.EventRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = true; // Mark as processed
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("EventRequests");
        }

        // View all events
        public async Task<IActionResult> AllEvents()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("AdminLogin", "Account");

            var events = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participations)
                .ToListAsync();
            return View(events);
        }

        // View all organizers
        public async Task<IActionResult> AllOrganizers()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("AdminLogin", "Account");

            var organizers = await _context.Organizers.ToListAsync();
            return View(organizers);
        }

        // View all participants
        public async Task<IActionResult> AllParticipants()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
                return RedirectToAction("AdminLogin", "Account");

            var participants = await _context.Participants.ToListAsync();
            return View(participants);
        }
    }
} 