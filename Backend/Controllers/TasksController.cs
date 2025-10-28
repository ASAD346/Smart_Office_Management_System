using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOMS.API.DTOs;
using SOMS.API.Services;

namespace SOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    private int GetEmployeeIdFromToken()
    {
        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
        return int.Parse(employeeIdClaim ?? "0");
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] int? employeeId = null, [FromQuery] string? status = null)
    {
        var tasks = await _taskService.GetAllTasksAsync(employeeId, status);
        return Ok(tasks);
    }

    // GET: api/tasks/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound(new { message = "Task not found" });

        return Ok(task);
    }

    // GET: api/tasks/my-tasks
    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var employeeId = GetEmployeeIdFromToken();
        var tasks = await _taskService.GetMyTasksAsync(employeeId);
        return Ok(tasks);
    }

    // GET: api/tasks/overdue
    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueTasks([FromQuery] int? employeeId = null)
    {
        var tasks = await _taskService.GetOverdueTasksAsync(employeeId);
        return Ok(tasks);
    }

    // POST: api/tasks
    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employeeId = GetEmployeeIdFromToken();
        var task = await _taskService.CreateTaskAsync(dto, employeeId);
        
        return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskId }, task);
    }

    // PUT: api/tasks/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = await _taskService.UpdateTaskAsync(id, dto);
        if (task == null)
            return NotFound(new { message = "Task not found" });

        return Ok(task);
    }

    // PUT: api/tasks/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskService.UpdateTaskStatusAsync(id, dto.Status);
        if (!result)
            return NotFound(new { message = "Task not found" });

        return Ok(new { message = "Task status updated successfully" });
    }

    // DELETE: api/tasks/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
        if (!result)
            return NotFound(new { message = "Task not found" });

        return Ok(new { message = "Task deleted successfully" });
    }
}
