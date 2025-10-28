using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOMS.API.Models
{
    [Table("MeetingAttendees")]
    public class MeetingAttendee
    {
        [Key]
        public int MeetingAttendeeId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Invited";

        // Navigation Properties
        [ForeignKey("MeetingId")]
        public virtual Meeting? Meeting { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
