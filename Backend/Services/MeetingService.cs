using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;
using SOMS.API.Models;

namespace SOMS.API.Services;

public class MeetingService
{
    private readonly ApplicationDbContext _context;

    public MeetingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MeetingDto>> GetAllMeetingsAsync(int? employeeId = null)
    {
        var query = _context.Meetings
            .Include(m => m.Creator)
            .Include(m => m.Attendees)
                .ThenInclude(a => a.Employee)
            .AsQueryable();

        // Filter by employee if specified
        if (employeeId.HasValue)
        {
            query = query.Where(m => m.Attendees.Any(a => a.EmployeeId == employeeId.Value) || m.CreatedBy == employeeId.Value);
        }

        var meetings = await query
            .OrderByDescending(m => m.MeetingDate)
            .ToListAsync();

        return meetings.Select(m => MapToDto(m)).ToList();
    }

    public async Task<MeetingDto?> GetMeetingByIdAsync(int meetingId)
    {
        var meeting = await _context.Meetings
            .Include(m => m.Creator)
            .Include(m => m.Attendees)
                .ThenInclude(a => a.Employee)
            .FirstOrDefaultAsync(m => m.MeetingId == meetingId);

        return meeting != null ? MapToDto(meeting) : null;
    }

    public async Task<MeetingDto> CreateMeetingAsync(CreateMeetingDto dto, int createdByEmployeeId)
    {
        var meeting = new Meeting
        {
            Title = dto.Title,
            Description = dto.Description,
            MeetingDate = dto.MeetingDate,
            Duration = dto.Duration,
            Location = dto.Location,
            CreatedBy = createdByEmployeeId,
            CreatedAt = DateTime.Now
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Add attendees
        if (dto.AttendeeIds.Any())
        {
            var attendees = dto.AttendeeIds.Select(id => new MeetingAttendee
            {
                MeetingId = meeting.MeetingId,
                EmployeeId = id,
                Status = "Invited"
            }).ToList();

            _context.MeetingAttendees.AddRange(attendees);
            await _context.SaveChangesAsync();
        }

        return (await GetMeetingByIdAsync(meeting.MeetingId))!;
    }

    public async Task<MeetingDto?> UpdateMeetingAsync(int meetingId, UpdateMeetingDto dto)
    {
        var meeting = await _context.Meetings
            .Include(m => m.Attendees)
            .FirstOrDefaultAsync(m => m.MeetingId == meetingId);

        if (meeting == null) return null;

        // Update fields if provided
        if (!string.IsNullOrEmpty(dto.Title))
            meeting.Title = dto.Title;
        
        if (dto.Description != null)
            meeting.Description = dto.Description;
        
        if (dto.MeetingDate.HasValue)
            meeting.MeetingDate = dto.MeetingDate.Value;
        
        if (dto.Duration.HasValue)
            meeting.Duration = dto.Duration.Value;
        
        if (dto.Location != null)
            meeting.Location = dto.Location;

        // Update attendees if provided
        if (dto.AttendeeIds != null)
        {
            // Remove existing attendees
            _context.MeetingAttendees.RemoveRange(meeting.Attendees);

            // Add new attendees
            var newAttendees = dto.AttendeeIds.Select(id => new MeetingAttendee
            {
                MeetingId = meetingId,
                EmployeeId = id,
                Status = "Invited"
            }).ToList();

            _context.MeetingAttendees.AddRange(newAttendees);
        }

        await _context.SaveChangesAsync();
        return await GetMeetingByIdAsync(meetingId);
    }

    public async Task<bool> DeleteMeetingAsync(int meetingId)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null) return false;

        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAttendeeStatusAsync(int meetingId, int employeeId, string status)
    {
        var attendee = await _context.MeetingAttendees
            .FirstOrDefaultAsync(a => a.MeetingId == meetingId && a.EmployeeId == employeeId);

        if (attendee == null) return false;

        attendee.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MeetingDto>> GetUpcomingMeetingsAsync(int employeeId)
    {
        var now = DateTime.Now;
        var meetings = await _context.Meetings
            .Include(m => m.Creator)
            .Include(m => m.Attendees)
                .ThenInclude(a => a.Employee)
            .Where(m => m.MeetingDate >= now && 
                       (m.Attendees.Any(a => a.EmployeeId == employeeId) || m.CreatedBy == employeeId))
            .OrderBy(m => m.MeetingDate)
            .ToListAsync();

        return meetings.Select(m => MapToDto(m)).ToList();
    }

    private MeetingDto MapToDto(Meeting meeting)
    {
        return new MeetingDto
        {
            MeetingId = meeting.MeetingId,
            Title = meeting.Title,
            Description = meeting.Description,
            MeetingDate = meeting.MeetingDate,
            Duration = meeting.Duration,
            Location = meeting.Location,
            CreatedBy = meeting.CreatedBy,
            CreatedByName = $"{meeting.Creator.FirstName} {meeting.Creator.LastName}",
            CreatedAt = meeting.CreatedAt,
            Attendees = meeting.Attendees.Select(a => new MeetingAttendeeDto
            {
                EmployeeId = a.EmployeeId,
                EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Status = a.Status
            }).ToList()
        };
    }
}
