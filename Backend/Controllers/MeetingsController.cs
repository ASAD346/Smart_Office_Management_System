using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOMS.API.DTOs;
using SOMS.API.Services;
using System.Security.Claims;

namespace SOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingsController : ControllerBase
{
    private readonly MeetingService _meetingService;

    public MeetingsController(MeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    private int GetEmployeeIdFromToken()
    {
        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
        return int.Parse(employeeIdClaim ?? "0");
    }

    // GET: api/meetings
    [HttpGet]
    public async Task<IActionResult> GetAllMeetings([FromQuery] int? employeeId = null)
    {
        var meetings = await _meetingService.GetAllMeetingsAsync(employeeId);
        return Ok(meetings);
    }

    // GET: api/meetings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeetingById(int id)
    {
        var meeting = await _meetingService.GetMeetingByIdAsync(id);
        if (meeting == null)
            return NotFound(new { message = "Meeting not found" });

        return Ok(meeting);
    }

    // GET: api/meetings/upcoming
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingMeetings()
    {
        var employeeId = GetEmployeeIdFromToken();
        var meetings = await _meetingService.GetUpcomingMeetingsAsync(employeeId);
        return Ok(meetings);
    }

    // GET: api/meetings/my-meetings
    [HttpGet("my-meetings")]
    public async Task<IActionResult> GetMyMeetings()
    {
        var employeeId = GetEmployeeIdFromToken();
        var meetings = await _meetingService.GetAllMeetingsAsync(employeeId);
        return Ok(meetings);
    }

    // POST: api/meetings
    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employeeId = GetEmployeeIdFromToken();
        var meeting = await _meetingService.CreateMeetingAsync(dto, employeeId);
        
        return CreatedAtAction(nameof(GetMeetingById), new { id = meeting.MeetingId }, meeting);
    }

    // PUT: api/meetings/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateMeeting(int id, [FromBody] UpdateMeetingDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var meeting = await _meetingService.UpdateMeetingAsync(id, dto);
        if (meeting == null)
            return NotFound(new { message = "Meeting not found" });

        return Ok(meeting);
    }

    // DELETE: api/meetings/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> DeleteMeeting(int id)
    {
        var result = await _meetingService.DeleteMeetingAsync(id);
        if (!result)
            return NotFound(new { message = "Meeting not found" });

        return Ok(new { message = "Meeting deleted successfully" });
    }

    // PUT: api/meetings/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateAttendeeStatus(int id, [FromBody] UpdateAttendeeStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employeeId = GetEmployeeIdFromToken();
        var result = await _meetingService.UpdateAttendeeStatusAsync(id, employeeId, dto.Status);
        
        if (!result)
            return NotFound(new { message = "Meeting or attendee not found" });

        return Ok(new { message = "Status updated successfully" });
    }
}
