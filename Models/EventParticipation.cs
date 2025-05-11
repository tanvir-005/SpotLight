using System.ComponentModel.DataAnnotations;

namespace SpotLight.Models
{
    public class EventParticipation
    {
        [Key]
        public int ParticipationId { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }

        public int ParticipantId { get; set; }

        public Participant Participant { get; set; }

        public bool Status { get; set; }
    }
} 