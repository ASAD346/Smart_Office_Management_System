using System.ComponentModel.DataAnnotations;

namespace SOMS.API.DTOs
{
    public class CheckOutDto
    {
        [Required]
        public int EmployeeId { get; set; }

        public string? CheckOutLocation { get; set; }

        public string? Notes { get; set; }
    }
}
