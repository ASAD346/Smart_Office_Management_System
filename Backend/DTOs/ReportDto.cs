namespace SOMS.API.DTOs;

public class AttendanceReportDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int LateDays { get; set; }
    public int AbsentDays { get; set; }
    public int HalfDays { get; set; }
    public double AttendancePercentage { get; set; }
    public double TotalHoursWorked { get; set; }
}

public class TaskAnalyticsDto
{
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int CancelledTasks { get; set; }
    public double CompletionRate { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskByEmployeeDto> TasksByEmployee { get; set; } = new();
}

public class TaskByEmployeeDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int AssignedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double CompletionRate { get; set; }
}

public class DailyAttendanceDto
{
    public DateTime Date { get; set; }
    public int TotalEmployees { get; set; }
    public int Present { get; set; }
    public int Late { get; set; }
    public int Absent { get; set; }
    public int HalfDay { get; set; }
    public double AttendanceRate { get; set; }
}

public class EmployeePerformanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public double AttendanceRate { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksAssigned { get; set; }
    public double TaskCompletionRate { get; set; }
    public int MeetingsAttended { get; set; }
}
