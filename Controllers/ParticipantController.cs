using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotLight.Models;
using SpotLight.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpotLight.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParticipantController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Participant")
                return RedirectToAction("ParticipantLogin", "Account");
            return View();
        }

        // View upcoming events
        public async Task<IActionResult> UpcomingEvents()
        {
            if (HttpContext.Session.GetString("UserType") != "Participant")
                return RedirectToAction("ParticipantLogin", "Account");

            var events = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participations)
                .Where(e => e.Date >= DateTime.Today)
                .ToListAsync();

            return View(events);
        }

        // Request to participate in an event
        [HttpPost]
        public async Task<IActionResult> RequestParticipation(int eventId)
        {
            if (HttpContext.Session.GetString("UserType") != "Participant")
                return RedirectToAction("ParticipantLogin", "Account");

            var event_ = await _context.Events
                .Include(e => e.Participations)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (event_ == null)
                return NotFound();

            // Check if there are available seats
            if (event_.Participations.Count(p => p.Status) >= event_.TotalSeats)
            {
                TempData["Error"] = "Sorry, this event is full.";
                return RedirectToAction("UpcomingEvents");
            }

            // Check if participant already has a request
            var existingRequest = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.EventId == eventId && 
                                        p.ParticipantId == HttpContext.Session.GetInt32("UserId"));

            if (existingRequest == null)
            {
                var participation = new EventParticipation
                {
                    EventId = eventId,
                    ParticipantId = HttpContext.Session.GetInt32("UserId").Value,
                    Status = false
                };

                _context.EventParticipations.Add(participation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UpcomingEvents");
        }

        // View enrolled events
        public async Task<IActionResult> MyEvents()
        {
            if (HttpContext.Session.GetString("UserType") != "Participant")
                return RedirectToAction("ParticipantLogin", "Account");

            var participantId = HttpContext.Session.GetInt32("UserId").Value;
            var participations = await _context.EventParticipations
                .Include(p => p.Event)
                    .ThenInclude(e => e.Organizer)
                .Where(p => p.ParticipantId == participantId && p.Status)
                .ToListAsync();

            return View(participations);
        }

        // View pending participation requests
        public async Task<IActionResult> PendingRequests()
        {
            if (HttpContext.Session.GetString("UserType") != "Participant")
                return RedirectToAction("ParticipantLogin", "Account");

            var participantId = HttpContext.Session.GetInt32("UserId").Value;
            var participations = await _context.EventParticipations
                .Include(p => p.Event)
                    .ThenInclude(e => e.Organizer)
                .Where(p => p.ParticipantId == participantId && !p.Status)
                .ToListAsync();

            return View(participations);
        }
    }
} 