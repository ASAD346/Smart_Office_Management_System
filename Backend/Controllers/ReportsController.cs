using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOMS.API.Services;
using System.Text;

namespace SOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: api/reports/attendance
    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendanceReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] int? departmentId = null)
    {
        var report = await _reportService.GetAttendanceReportAsync(startDate, endDate, departmentId);
        return Ok(report);
    }

    // GET: api/reports/tasks
    [HttpGet("tasks")]
    public async Task<IActionResult> GetTaskAnalytics(
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        var analytics = await _reportService.GetTaskAnalyticsAsync(startDate, endDate);
        return Ok(analytics);
    }

    // GET: api/reports/daily-attendance
    [HttpGet("daily-attendance")]
    public async Task<IActionResult> GetDailyAttendance(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var report = await _reportService.GetDailyAttendanceAsync(startDate, endDate);
        return Ok(report);
    }

    // GET: api/reports/employee-performance
    [HttpGet("employee-performance")]
    public async Task<IActionResult> GetEmployeePerformance(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var performance = await _reportService.GetEmployeePerformanceAsync(startDate, endDate);
        return Ok(performance);
    }

    // GET: api/reports/attendance/export
    [HttpGet("attendance/export")]
    public async Task<IActionResult> ExportAttendanceReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var csv = await _reportService.ExportAttendanceReportToCsvAsync(startDate, endDate);
        
        var bytes = Encoding.UTF8.GetBytes(csv);
        var fileName = $"AttendanceReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";
        
        return File(bytes, "text/csv", fileName);
    }

    // GET: api/reports/tasks/export
    [HttpGet("tasks/export")]
    public async Task<IActionResult> ExportTaskAnalytics(
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        var csv = await _reportService.ExportTaskAnalyticsToCsvAsync(startDate, endDate);
        
        var bytes = Encoding.UTF8.GetBytes(csv);
        var fileName = $"TaskAnalytics_{DateTime.Now:yyyyMMddHHmmss}.csv";
        
        return File(bytes, "text/csv", fileName);
    }
}
