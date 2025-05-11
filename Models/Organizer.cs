using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace SpotLight.Models
{
    public class Organizer
    {
        [Key]
        public int OrganizerId { get; set; }

        [Required]
        public string InstitutionName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<EventRequest> EventRequests { get; set; } = new List<EventRequest>();
    }
}
