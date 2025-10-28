using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOMS.API.Models
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int AssignedTo { get; set; }

        [Required]
        public int AssignedBy { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("AssignedTo")]
        public virtual Employee? AssignedToEmployee { get; set; }

        [ForeignKey("AssignedBy")]
        public virtual Employee? AssignedByEmployee { get; set; }
    }
}
