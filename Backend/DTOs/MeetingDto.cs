namespace SOMS.API.DTOs;

public class MeetingDto
{
    public int MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime MeetingDate { get; set; }
    public int Duration { get; set; } // in minutes
    public string? Location { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<MeetingAttendeeDto> Attendees { get; set; } = new();
}

public class MeetingAttendeeDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Status { get; set; } = "Invited"; // Invited, Accepted, Declined, Attended
}

public class CreateMeetingDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime MeetingDate { get; set; }
    public int Duration { get; set; } // in minutes
    public string? Location { get; set; }
    public List<int> AttendeeIds { get; set; } = new();
}

public class UpdateMeetingDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? MeetingDate { get; set; }
    public int? Duration { get; set; }
    public string? Location { get; set; }
    public List<int>? AttendeeIds { get; set; }
}

public class UpdateAttendeeStatusDto
{
    public string Status { get; set; } = string.Empty; // Accepted, Declined, Attended
}
