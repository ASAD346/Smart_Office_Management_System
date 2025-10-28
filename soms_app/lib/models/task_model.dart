class Task {
  final int taskId;
  final String title;
  final String? description;
  final String status;
  final String priority;
  final DateTime? startDate;
  final DateTime? dueDate;
  final DateTime? completedDate;
  final int assignedTo;
  final int assignedBy;
  final String assignedToName;
  final String assignedByName;
  final DateTime createdAt;

  Task({
    required this.taskId,
    required this.title,
    this.description,
    required this.status,
    required this.priority,
    this.startDate,
    this.dueDate,
    this.completedDate,
    required this.assignedTo,
    required this.assignedBy,
    required this.assignedToName,
    required this.assignedByName,
    required this.createdAt,
  });

  factory Task.fromJson(Map<String, dynamic> json) {
    return Task(
      taskId: json['taskId'],
      title: json['title'],
      description: json['description'],
      status: json['status'],
      priority: json['priority'],
      startDate: json['startDate'] != null 
          ? DateTime.parse(json['startDate']) 
          : null,
      dueDate: json['dueDate'] != null 
          ? DateTime.parse(json['dueDate']) 
          : null,
      completedDate: json['completedDate'] != null 
          ? DateTime.parse(json['completedDate']) 
          : null,
      assignedTo: json['assignedTo'],
      assignedBy: json['assignedBy'],
      assignedToName: json['assignedToName'],
      assignedByName: json['assignedByName'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }

  bool get isOverdue {
    if (dueDate == null || status == 'Completed') return false;
    return DateTime.now().isAfter(dueDate!);
  }
}
