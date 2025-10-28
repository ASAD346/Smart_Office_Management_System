using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;

namespace SOMS.API.Services;

public class TaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskDto>> GetAllTasksAsync(int? employeeId = null, string? status = null)
    {
        var query = _context.Tasks
            .Include(t => t.AssignedToEmployee)
            .Include(t => t.AssignedByEmployee)
            .AsQueryable();

        // Filter by employee if specified
        if (employeeId.HasValue)
        {
            query = query.Where(t => t.AssignedTo == employeeId.Value || t.AssignedBy == employeeId.Value);
        }

        // Filter by status if specified
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status == status);
        }

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => MapToDto(t)).ToList();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedToEmployee)
            .Include(t => t.AssignedByEmployee)
            .FirstOrDefaultAsync(t => t.TaskId == taskId);

        return task != null ? MapToDto(task) : null;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int assignedByEmployeeId)
    {
        var task = new Models.Task
        {
            Title = dto.Title,
            Description = dto.Description,
            AssignedTo = dto.AssignedTo,
            AssignedBy = assignedByEmployeeId,
            Status = "Pending",
            Priority = dto.Priority,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return (await GetTaskByIdAsync(task.TaskId))!;
    }

    public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return null;

        // Update fields if provided
        if (!string.IsNullOrEmpty(dto.Title))
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.AssignedTo.HasValue)
            task.AssignedTo = dto.AssignedTo.Value;

        if (!string.IsNullOrEmpty(dto.Status))
        {
            task.Status = dto.Status;
            
            // Set completion date if task is completed
            if (dto.Status == "Completed")
                task.CompletedDate = DateTime.Now;
        }

        if (!string.IsNullOrEmpty(dto.Priority))
            task.Priority = dto.Priority;

        if (dto.StartDate.HasValue)
            task.StartDate = dto.StartDate;

        if (dto.DueDate.HasValue)
            task.DueDate = dto.DueDate;

        task.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return await GetTaskByIdAsync(taskId);
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, string status)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.Status = status;
        task.UpdatedAt = DateTime.Now;

        if (status == "Completed")
            task.CompletedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(int employeeId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.AssignedToEmployee)
            .Include(t => t.AssignedByEmployee)
            .Where(t => t.AssignedTo == employeeId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => MapToDto(t)).ToList();
    }

    public async Task<List<TaskDto>> GetOverdueTasksAsync(int? employeeId = null)
    {
        var now = DateTime.Now;
        var query = _context.Tasks
            .Include(t => t.AssignedToEmployee)
            .Include(t => t.AssignedByEmployee)
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now && 
                       (t.Status == "Pending" || t.Status == "In Progress"));

        if (employeeId.HasValue)
        {
            query = query.Where(t => t.AssignedTo == employeeId.Value);
        }

        var tasks = await query.ToListAsync();
        return tasks.Select(t => MapToDto(t)).ToList();
    }

    private TaskDto MapToDto(Models.Task task)
    {
        return new TaskDto
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            AssignedTo = task.AssignedTo,
            AssignedToName = $"{task.AssignedToEmployee.FirstName} {task.AssignedToEmployee.LastName}",
            AssignedBy = task.AssignedBy,
            AssignedByName = $"{task.AssignedByEmployee.FirstName} {task.AssignedByEmployee.LastName}",
            Status = task.Status,
            Priority = task.Priority,
            StartDate = task.StartDate,
            DueDate = task.DueDate,
            CompletedDate = task.CompletedDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
