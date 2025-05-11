using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpotLight.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        public int OrganizerId { get; set; }

        public Organizer Organizer { get; set; }

        public ICollection<EventParticipation> Participations { get; set; } = new List<EventParticipation>();
    }
}
