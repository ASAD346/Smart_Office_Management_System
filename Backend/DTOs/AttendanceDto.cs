namespace SOMS.API.DTOs
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckInLocation { get; set; }
        public string? CheckOutLocation { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public TimeSpan? WorkDuration { get; set; }
    }
}
