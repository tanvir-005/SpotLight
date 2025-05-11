using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotLight.Models;
using SpotLight.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpotLight.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Organizer")
                return RedirectToAction("OrganizerLogin", "Account");
            return View();
        }

        // Create event request
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("UserType") != "Organizer")
                return RedirectToAction("OrganizerLogin", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventRequest request)
        {
            if (ModelState.IsValid)
            {
                request.OrganizerId = HttpContext.Session.GetInt32("UserId").Value;
                request.Status = false;
                _context.EventRequests.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyEvents");
            }
            return View(request);
        }

        // View organizer's events
        public async Task<IActionResult> MyEvents()
        {
            if (HttpContext.Session.GetString("UserType") != "Organizer")
                return RedirectToAction("OrganizerLogin", "Account");

            var organizerId = HttpContext.Session.GetInt32("UserId").Value;
            var events = await _context.Events
                .Include(e => e.Participations)
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
            return View(events);
        }

        // Edit event
        public async Task<IActionResult> EditEvent(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "Organizer")
                return RedirectToAction("OrganizerLogin", "Account");

            var event_ = await _context.Events.FindAsync(id);
            if (event_ == null || event_.OrganizerId != HttpContext.Session.GetInt32("UserId"))
                return NotFound();

            return View(event_);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(Event event_)
        {
            if (ModelState.IsValid)
            {
                _context.Update(event_);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyEvents");
            }
            return View(event_);
        }

        // View participation requests for an event
        public async Task<IActionResult> ParticipationRequests(int eventId)
        {
            if (HttpContext.Session.GetString("UserType") != "Organizer")
                return RedirectToAction("OrganizerLogin", "Account");

            var event_ = await _context.Events
                .Include(e => e.Participations)
                    .ThenInclude(p => p.Participant)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (event_ == null || event_.OrganizerId != HttpContext.Session.GetInt32("UserId"))
                return NotFound();

            return View(event_);
        }

        // Approve participation request
        [HttpPost]
        public async Task<IActionResult> ApproveParticipation(int eventId, int participantId)
        {
            var event_ = await _context.Events
                .Include(e => e.Participations)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (event_ == null || event_.OrganizerId != HttpContext.Session.GetInt32("UserId"))
                return NotFound();

            if (event_.Participations.Count(p => p.Status) < event_.TotalSeats)
            {
                var participation = await _context.EventParticipations
                    .FirstOrDefaultAsync(p => p.EventId == eventId && p.ParticipantId == participantId);

                if (participation != null)
                {
                    participation.Status = true;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("ParticipationRequests", new { eventId });
        }

        // Reject participation request
        [HttpPost]
        public async Task<IActionResult> RejectParticipation(int eventId, int participantId)
        {
            var participation = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.ParticipantId == participantId);

            if (participation != null)
            {
                participation.Status = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ParticipationRequests", new { eventId });
        }
    }
} 