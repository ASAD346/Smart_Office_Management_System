using System.ComponentModel.DataAnnotations;

namespace SOMS.API.DTOs
{
    public class CheckInDto
    {
        [Required]
        public int EmployeeId { get; set; }

        public string? CheckInLocation { get; set; }

        public string? Notes { get; set; }
    }
}
