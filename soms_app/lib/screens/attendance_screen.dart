import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../services/api_service.dart';
import '../services/auth_service.dart';
import '../models/attendance_model.dart';

class AttendanceScreen extends StatefulWidget {
  const AttendanceScreen({super.key});

  @override
  State<AttendanceScreen> createState() => _AttendanceScreenState();
}

class _AttendanceScreenState extends State<AttendanceScreen> {
  bool _isLoading = false;
  Attendance? _todayAttendance;
  final _locationController = TextEditingController(text: 'Office');

  @override
  void initState() {
    super.initState();
    _loadTodayAttendance();
  }

  @override
  void dispose() {
    _locationController.dispose();
    super.dispose();
  }

  Future<void> _loadTodayAttendance() async {
    setState(() => _isLoading = true);
    try {
      final user = await AuthService.getCurrentUser();
      if (user?.employeeId == null) return;

      final attendances = await ApiService.getTodayAttendance();
      final myAttendance = attendances.where((a) => a.employeeId == user!.employeeId).firstOrNull;

      setState(() => _todayAttendance = myAttendance);
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

  Future<void> _checkIn() async {
    final user = await AuthService.getCurrentUser();
    if (user?.employeeId == null) return;

    setState(() => _isLoading = true);
    try {
      await ApiService.checkIn(user!.employeeId!, _locationController.text);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Checked in successfully!'), backgroundColor: Colors.green),
        );
      }
      await _loadTodayAttendance();
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

  Future<void> _checkOut() async {
    final user = await AuthService.getCurrentUser();
    if (user?.employeeId == null) return;

    setState(() => _isLoading = true);
    try {
      await ApiService.checkOut(user!.employeeId!, _locationController.text);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Checked out successfully!'), backgroundColor: Colors.green),
        );
      }
      await _loadTodayAttendance();
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: RefreshIndicator(
        onRefresh: _loadTodayAttendance,
        child: _isLoading
            ? const Center(child: CircularProgressIndicator())
            : SingleChildScrollView(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Card(
                      elevation: 4,
                      child: Padding(
                        padding: const EdgeInsets.all(16),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Today\'s Attendance',
                              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                                    fontWeight: FontWeight.bold,
                                  ),
                            ),
                            const SizedBox(height: 8),
                            Text(
                              DateFormat('EEEE, MMMM d, y').format(DateTime.now()),
                              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                    color: Colors.grey[600],
                                  ),
                            ),
                            const Divider(height: 24),
                            if (_todayAttendance == null) ...[
                              const Text('You haven\'t checked in today'),
                              const SizedBox(height: 16),
                              TextField(
                                controller: _locationController,
                                decoration: const InputDecoration(
                                  labelText: 'Location',
                                  border: OutlineInputBorder(),
                                ),
                              ),
                              const SizedBox(height: 16),
                              SizedBox(
                                width: double.infinity,
                                child: ElevatedButton.icon(
                                  onPressed: _checkIn,
                                  icon: const Icon(Icons.login),
                                  label: const Text('Check In'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.green,
                                    padding: const EdgeInsets.all(16),
                                  ),
                                ),
                              ),
                            ] else ...[
                              _buildInfoRow('Check-in Time', DateFormat('HH:mm').format(_todayAttendance!.checkInTime)),
                              _buildInfoRow('Location', _todayAttendance!.checkInLocation ?? '-'),
                              _buildInfoRow('Status', _todayAttendance!.status),
                              if (_todayAttendance!.checkOutTime != null) ...[
                                const Divider(height: 24),
                                _buildInfoRow('Check-out Time', DateFormat('HH:mm').format(_todayAttendance!.checkOutTime!)),
                                _buildInfoRow('Duration', _todayAttendance!.duration),
                              ] else ...[
                                const SizedBox(height: 16),
                                TextField(
                                  controller: _locationController,
                                  decoration: const InputDecoration(
                                    labelText: 'Location',
                                    border: OutlineInputBorder(),
                                  ),
                                ),
                                const SizedBox(height: 16),
                                SizedBox(
                                  width: double.infinity,
                                  child: ElevatedButton.icon(
                                    onPressed: _checkOut,
                                    icon: const Icon(Icons.logout),
                                    label: const Text('Check Out'),
                                    style: ElevatedButton.styleFrom(
                                      backgroundColor: Colors.orange,
                                      padding: const EdgeInsets.all(16),
                                    ),
                                  ),
                                ),
                              ],
                            ],
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(fontWeight: FontWeight.w500)),
          Text(value),
        ],
      ),
    );
  }
}
