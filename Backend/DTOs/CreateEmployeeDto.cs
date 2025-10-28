using System.ComponentModel.DataAnnotations;

namespace SOMS.API.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public int? DepartmentId { get; set; }

        public string? Position { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        public string? Address { get; set; }

        [Required]
        public int RoleId { get; set; } = 3; // Default to Employee role
    }
}
