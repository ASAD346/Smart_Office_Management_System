using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;
using SOMS.API.Models;

namespace SOMS.API.Services
{
    public class AttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceDto?> CheckInAsync(CheckInDto dto)
        {
            // Check if employee already checked in today
            var today = DateTime.Today;
            var existingAttendance = await _context.Attendances
                .Where(a => a.EmployeeId == dto.EmployeeId && 
                           a.CheckInTime.Date == today &&
                           a.CheckOutTime == null)
                .FirstOrDefaultAsync();

            if (existingAttendance != null)
                return null; // Already checked in

            var attendance = new Attendance
            {
                EmployeeId = dto.EmployeeId,
                CheckInTime = DateTime.Now,
                CheckInLocation = dto.CheckInLocation,
                Status = "Present",
                Notes = dto.Notes
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return await GetAttendanceByIdAsync(attendance.AttendanceId);
        }

        public async Task<AttendanceDto?> CheckOutAsync(CheckOutDto dto)
        {
            // Find today's attendance record for the employee that hasn't been checked out yet
            var today = DateTime.Today;
            var attendance = await _context.Attendances
                .Where(a => a.EmployeeId == dto.EmployeeId && 
                           a.CheckInTime.Date == today &&
                           a.CheckOutTime == null)
                .FirstOrDefaultAsync();

            if (attendance == null)
                return null; // Not found or already checked out

            attendance.CheckOutTime = DateTime.Now;
            attendance.CheckOutLocation = dto.CheckOutLocation;
            
            if (!string.IsNullOrEmpty(dto.Notes))
                attendance.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return await GetAttendanceByIdAsync(attendance.AttendanceId);
        }

        public async Task<AttendanceDto?> GetAttendanceByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.AttendanceId == id)
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee!.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    Status = a.Status,
                    Notes = a.Notes,
                    WorkDuration = a.CheckOutTime.HasValue 
                        ? a.CheckOutTime.Value - a.CheckInTime 
                        : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<AttendanceDto>> GetTodayAttendanceAsync()
        {
            var today = DateTime.Today;
            
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.CheckInTime.Date == today)
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee!.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    Status = a.Status,
                    Notes = a.Notes,
                    WorkDuration = a.CheckOutTime.HasValue 
                        ? a.CheckOutTime.Value - a.CheckInTime 
                        : null
                })
                .OrderBy(a => a.CheckInTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceDto>> GetAttendanceByDateAsync(DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.CheckInTime.Date == date.Date)
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee!.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    Status = a.Status,
                    Notes = a.Notes,
                    WorkDuration = a.CheckOutTime.HasValue 
                        ? a.CheckOutTime.Value - a.CheckInTime 
                        : null
                })
                .OrderBy(a => a.CheckInTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceDto>> GetEmployeeAttendanceAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId);

            if (startDate.HasValue)
                query = query.Where(a => a.CheckInTime.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.CheckInTime.Date <= endDate.Value.Date);

            return await query
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee!.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    Status = a.Status,
                    Notes = a.Notes,
                    WorkDuration = a.CheckOutTime.HasValue 
                        ? a.CheckOutTime.Value - a.CheckInTime 
                        : null
                })
                .OrderByDescending(a => a.CheckInTime)
                .ToListAsync();
        }
    }
}
