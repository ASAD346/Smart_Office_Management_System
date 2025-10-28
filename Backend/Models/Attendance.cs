using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOMS.API.Models
{
    [Table("Attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [StringLength(255)]
        public string? CheckInLocation { get; set; }

        [StringLength(255)]
        public string? CheckOutLocation { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Present";

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
