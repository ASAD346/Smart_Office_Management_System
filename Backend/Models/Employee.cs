using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOMS.API.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public int? DepartmentId { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        [StringLength(255)]
        public string? ProfilePicture { get; set; }

        public string? Address { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Meeting> CreatedMeetings { get; set; } = new List<Meeting>();
        public virtual ICollection<MeetingAttendee> MeetingAttendees { get; set; } = new List<MeetingAttendee>();
        public virtual ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Task> CreatedTasks { get; set; } = new List<Task>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
