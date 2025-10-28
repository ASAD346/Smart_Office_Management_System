using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOMS.API.DTOs;
using SOMS.API.Services;

namespace SOMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // POST: api/attendance/checkin
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendance = await _attendanceService.CheckInAsync(dto);

            if (attendance == null)
                return BadRequest(new { message = "Already checked in today or invalid employee" });

            return Ok(attendance);
        }

        // POST: api/attendance/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendance = await _attendanceService.CheckOutAsync(dto);

            if (attendance == null)
                return BadRequest(new { message = "Attendance not found or already checked out" });

            return Ok(attendance);
        }

        // GET: api/attendance/today
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayAttendance()
        {
            var attendances = await _attendanceService.GetTodayAttendanceAsync();
            return Ok(attendances);
        }

        // GET: api/attendance/date/2024-01-15
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetAttendanceByDate(DateTime date)
        {
            var attendances = await _attendanceService.GetAttendanceByDateAsync(date);
            return Ok(attendances);
        }

        // GET: api/attendance/employee/1
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAttendance(int employeeId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var attendances = await _attendanceService.GetEmployeeAttendanceAsync(employeeId, startDate, endDate);
            return Ok(attendances);
        }

        // GET: api/attendance/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            
            if (attendance == null)
                return NotFound(new { message = "Attendance record not found" });

            return Ok(attendance);
        }
    }
}
