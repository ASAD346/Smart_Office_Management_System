import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../services/api_service.dart';
import '../models/task_model.dart';

class TasksScreen extends StatefulWidget {
  const TasksScreen({super.key});

  @override
  State<TasksScreen> createState() => _TasksScreenState();
}

class _TasksScreenState extends State<TasksScreen> {
  List<Task> _tasks = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadTasks();
  }

  Future<void> _loadTasks() async {
    setState(() => _isLoading = true);
    try {
      final tasks = await ApiService.getMyTasks();
      setState(() => _tasks = tasks);
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error: ${e.toString()}'), backgroundColor: Colors.red),
        );
      }
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _updateStatus(int taskId, String currentStatus) async {
    final statuses = ['Pending', 'In Progress', 'Completed'];
    final newStatus = await showDialog<String>(
      context: context,
      builder: (context) => SimpleDialog(
        title: const Text('Update Status'),
        children: statuses
            .map((status) => SimpleDialogOption(
                  onPressed: () => Navigator.pop(context, status),
                  child: Text(status),
                ))
            .toList(),
      ),
    );

    if (newStatus != null && newStatus != currentStatus) {
      try {
        await ApiService.updateTaskStatus(taskId, newStatus);
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Status updated!'), backgroundColor: Colors.green),
          );
        }
        await _loadTasks();
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Error: ${e.toString()}'), backgroundColor: Colors.red),
          );
        }
      }
    }
  }

  Color _getStatusColor(String status) {
    switch (status) {
      case 'Completed':
        return Colors.green;
      case 'In Progress':
        return Colors.orange;
      default:
        return Colors.blue;
    }
  }

  Color _getPriorityColor(String priority) {
    switch (priority) {
      case 'High':
        return Colors.red;
      case 'Medium':
        return Colors.orange;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: RefreshIndicator(
        onRefresh: _loadTasks,
        child: _isLoading
            ? const Center(child: CircularProgressIndicator())
            : _tasks.isEmpty
                ? const Center(child: Text('No tasks assigned'))
                : ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: _tasks.length,
                    itemBuilder: (context, index) {
                      final task = _tasks[index];
                      return Card(
                        margin: const EdgeInsets.only(bottom: 16),
                        child: ListTile(
                          title: Text(
                            task.title,
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                          subtitle: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              if (task.description != null)
                                Text(task.description!, maxLines: 2, overflow: TextOverflow.ellipsis),
                              const SizedBox(height: 4),
                              Row(
                                children: [
                                  Chip(
                                    label: Text(task.status, style: const TextStyle(fontSize: 12)),
                                    backgroundColor: _getStatusColor(task.status).withOpacity(0.2),
                                    labelPadding: EdgeInsets.zero,
                                    padding: const EdgeInsets.symmetric(horizontal: 8),
                                  ),
                                  const SizedBox(width: 8),
                                  Chip(
                                    label: Text(task.priority, style: const TextStyle(fontSize: 12)),
                                    backgroundColor: _getPriorityColor(task.priority).withOpacity(0.2),
                                    labelPadding: EdgeInsets.zero,
                                    padding: const EdgeInsets.symmetric(horizontal: 8),
                                  ),
                                ],
                              ),
                              if (task.dueDate != null)
                                Text(
                                  'Due: ${DateFormat('MMM d, y').format(task.dueDate!)}',
                                  style: TextStyle(
                                    color: task.isOverdue ? Colors.red : Colors.grey[600],
                                    fontSize: 12,
                                  ),
                                ),
                            ],
                          ),
                          trailing: IconButton(
                            icon: const Icon(Icons.edit),
                            onPressed: () => _updateStatus(task.taskId, task.status),
                          ),
                          isThreeLine: true,
                        ),
                      );
                    },
                  ),
      ),
    );
  }
}
