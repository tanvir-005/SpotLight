using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpotLight.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Contact { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public ICollection<EventParticipation> Participations { get; set; } = new List<EventParticipation>();
    }
}
