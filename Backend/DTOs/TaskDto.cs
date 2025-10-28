namespace SOMS.API.DTOs;

public class TaskDto
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AssignedTo { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public int AssignedBy { get; set; }
    public string AssignedByName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Cancelled
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AssignedTo { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? AssignedTo { get; set; }
    public string? Status { get; set; } // Pending, In Progress, Completed, Cancelled
    public string? Priority { get; set; } // Low, Medium, High, Urgent
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskStatusDto
{
    public string Status { get; set; } = string.Empty; // In Progress, Completed, Cancelled
}
