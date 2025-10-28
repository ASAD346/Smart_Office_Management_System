using System.ComponentModel.DataAnnotations;

namespace SOMS.API.DTOs
{
    public class UpdateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public int? DepartmentId { get; set; }

        public string? Position { get; set; }

        public string? Address { get; set; }

        public bool? IsActive { get; set; }
    }
}
