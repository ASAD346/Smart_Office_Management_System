namespace SOMS.API.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? Position { get; set; }
        public DateTime JoinDate { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
