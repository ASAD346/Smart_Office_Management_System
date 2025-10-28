using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;
using System.Text;

namespace SOMS.API.Services;

public class ReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceReportDto>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null)
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Attendances)
            .Where(e => departmentId == null || e.DepartmentId == departmentId)
            .ToListAsync();

        var reports = new List<AttendanceReportDto>();

        foreach (var employee in employees)
        {
            var attendances = employee.Attendances
                .Where(a => a.CheckInTime >= startDate && a.CheckInTime <= endDate)
                .ToList();

            var totalDays = (endDate - startDate).Days + 1;
            var presentDays = attendances.Count(a => a.Status == "Present");
            var lateDays = attendances.Count(a => a.Status == "Late");
            var absentDays = totalDays - attendances.Count;
            var halfDays = attendances.Count(a => a.Status == "Half-Day");

            var totalHours = attendances
                .Where(a => a.CheckOutTime.HasValue)
                .Sum(a => (a.CheckOutTime!.Value - a.CheckInTime).TotalHours);

            reports.Add(new AttendanceReportDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Department = employee.Department?.DepartmentName ?? "N/A",
                TotalDays = totalDays,
                PresentDays = presentDays,
                LateDays = lateDays,
                AbsentDays = absentDays,
                HalfDays = halfDays,
                AttendancePercentage = totalDays > 0 ? Math.Round((double)attendances.Count / totalDays * 100, 2) : 0,
                TotalHoursWorked = Math.Round(totalHours, 2)
            });
        }

        return reports;
    }

    public async Task<TaskAnalyticsDto> GetTaskAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Tasks
            .Include(t => t.AssignedToEmployee)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        var tasks = await query.ToListAsync();
        var totalTasks = tasks.Count;
        var completedTasks = tasks.Count(t => t.Status == "Completed");
        var now = DateTime.Now;

        var tasksByEmployee = tasks
            .GroupBy(t => t.AssignedTo)
            .Select(g => new TaskByEmployeeDto
            {
                EmployeeId = g.Key,
                EmployeeName = $"{g.First().AssignedToEmployee.FirstName} {g.First().AssignedToEmployee.LastName}",
                AssignedTasks = g.Count(),
                CompletedTasks = g.Count(t => t.Status == "Completed"),
                CompletionRate = g.Count() > 0 ? Math.Round((double)g.Count(t => t.Status == "Completed") / g.Count() * 100, 2) : 0
            })
            .ToList();

        return new TaskAnalyticsDto
        {
            TotalTasks = totalTasks,
            PendingTasks = tasks.Count(t => t.Status == "Pending"),
            InProgressTasks = tasks.Count(t => t.Status == "In Progress"),
            CompletedTasks = completedTasks,
            CancelledTasks = tasks.Count(t => t.Status == "Cancelled"),
            CompletionRate = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 2) : 0,
            OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && 
                                           (t.Status == "Pending" || t.Status == "In Progress")),
            TasksByEmployee = tasksByEmployee
        };
    }

    public async Task<List<DailyAttendanceDto>> GetDailyAttendanceAsync(DateTime startDate, DateTime endDate)
    {
        var totalEmployees = await _context.Employees.CountAsync();
        var attendances = await _context.Attendances
            .Where(a => a.CheckInTime >= startDate && a.CheckInTime <= endDate)
            .ToListAsync();

        var dailyReports = attendances
            .GroupBy(a => a.CheckInTime.Date)
            .Select(g => new DailyAttendanceDto
            {
                Date = g.Key,
                TotalEmployees = totalEmployees,
                Present = g.Count(a => a.Status == "Present"),
                Late = g.Count(a => a.Status == "Late"),
                Absent = totalEmployees - g.Count(),
                HalfDay = g.Count(a => a.Status == "Half-Day"),
                AttendanceRate = totalEmployees > 0 ? Math.Round((double)g.Count() / totalEmployees * 100, 2) : 0
            })
            .OrderBy(d => d.Date)
            .ToList();

        return dailyReports;
    }

    public async Task<List<EmployeePerformanceDto>> GetEmployeePerformanceAsync(DateTime startDate, DateTime endDate)
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Attendances)
            .Include(e => e.AssignedTasks)
            .Include(e => e.MeetingAttendees)
            .ToListAsync();

        var performances = new List<EmployeePerformanceDto>();

        foreach (var employee in employees)
        {
            var attendances = employee.Attendances
                .Where(a => a.CheckInTime >= startDate && a.CheckInTime <= endDate)
                .ToList();

            var totalDays = (endDate - startDate).Days + 1;
            var attendanceRate = totalDays > 0 ? Math.Round((double)attendances.Count / totalDays * 100, 2) : 0;

            var tasks = employee.AssignedTasks
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .ToList();

            var tasksCompleted = tasks.Count(t => t.Status == "Completed");
            var tasksAssigned = tasks.Count;
            var taskCompletionRate = tasksAssigned > 0 ? Math.Round((double)tasksCompleted / tasksAssigned * 100, 2) : 0;

            var meetingsAttended = employee.MeetingAttendees
                .Count(ma => ma.Status == "Attended" && 
                            ma.Meeting.MeetingDate >= startDate && 
                            ma.Meeting.MeetingDate <= endDate);

            performances.Add(new EmployeePerformanceDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Department = employee.Department?.DepartmentName ?? "N/A",
                AttendanceRate = attendanceRate,
                TasksCompleted = tasksCompleted,
                TasksAssigned = tasksAssigned,
                TaskCompletionRate = taskCompletionRate,
                MeetingsAttended = meetingsAttended
            });
        }

        return performances;
    }

    public async Task<string> ExportAttendanceReportToCsvAsync(DateTime startDate, DateTime endDate)
    {
        var reports = await GetAttendanceReportAsync(startDate, endDate);
        
        var csv = new StringBuilder();
        csv.AppendLine("Employee ID,Employee Name,Department,Total Days,Present,Late,Absent,Half-Day,Attendance %,Total Hours");

        foreach (var report in reports)
        {
            csv.AppendLine($"{report.EmployeeId},{report.EmployeeName},{report.Department},{report.TotalDays}," +
                          $"{report.PresentDays},{report.LateDays},{report.AbsentDays},{report.HalfDays}," +
                          $"{report.AttendancePercentage},{report.TotalHoursWorked}");
        }

        return csv.ToString();
    }

    public async Task<string> ExportTaskAnalyticsToCsvAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var analytics = await GetTaskAnalyticsAsync(startDate, endDate);
        
        var csv = new StringBuilder();
        csv.AppendLine("Employee ID,Employee Name,Assigned Tasks,Completed Tasks,Completion Rate %");

        foreach (var employeeTask in analytics.TasksByEmployee)
        {
            csv.AppendLine($"{employeeTask.EmployeeId},{employeeTask.EmployeeName}," +
                          $"{employeeTask.AssignedTasks},{employeeTask.CompletedTasks},{employeeTask.CompletionRate}");
        }

        return csv.ToString();
    }
}
