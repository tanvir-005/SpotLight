using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotLight.Models;
using SpotLight.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SpotLight.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Login
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                HttpContext.Session.SetString("UserType", "Admin");
                return RedirectToAction("Index", "Admin");
            }
            ModelState.AddModelError("", "Invalid admin credentials");
            return View();
        }

        // Organizer Login
        public IActionResult OrganizerLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrganizerLogin(string email, string password)
        {
            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.Email == email && o.Password == password);

            if (organizer != null)
            {
                HttpContext.Session.SetString("UserType", "Organizer");
                HttpContext.Session.SetInt32("UserId", organizer.OrganizerId);
                return RedirectToAction("Index", "Organizer");
            }
            ModelState.AddModelError("", "Invalid organizer credentials");
            return View();
        }

        // Organizer Signup
        public IActionResult OrganizerSignup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrganizerSignup(Organizer organizer)
        {
            if (ModelState.IsValid)
            {
                _context.Organizers.Add(organizer);
                await _context.SaveChangesAsync();
                return RedirectToAction("OrganizerLogin");
            }
            return View(organizer);
        }

        // Participant Login
        public IActionResult ParticipantLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ParticipantLogin(string email, string password)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == email && p.Password == password);

            if (participant != null)
            {
                HttpContext.Session.SetString("UserType", "Participant");
                HttpContext.Session.SetInt32("UserId", participant.ParticipantId);
                return RedirectToAction("Index", "Participant");
            }
            ModelState.AddModelError("", "Invalid participant credentials");
            return View();
        }

        // Participant Signup
        public IActionResult ParticipantSignup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ParticipantSignup(Participant participant)
        {
            if (ModelState.IsValid)
            {
                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
                return RedirectToAction("ParticipantLogin");
            }
            return View(participant);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
} 