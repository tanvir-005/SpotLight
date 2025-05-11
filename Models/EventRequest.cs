using System;
using System.ComponentModel.DataAnnotations;

namespace SpotLight.Models
{
    public class EventRequest
    {
        [Key]
        public int RequestId { get; set; }

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
        public int NumberOfSeats { get; set; }

        public bool Status { get; set; }

        public int OrganizerId { get; set; }

        public Organizer Organizer { get; set; }
    }
} 