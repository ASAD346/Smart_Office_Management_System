using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOMS.API.Models
{
    [Table("Meetings")]
    public class Meeting
    {
        [Key]
        public int MeetingId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime MeetingDate { get; set; }

        [Required]
        public int Duration { get; set; } // in minutes

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("CreatedBy")]
        public virtual Employee? Creator { get; set; }

        public virtual ICollection<MeetingAttendee> Attendees { get; set; } = new List<MeetingAttendee>();
    }
}
