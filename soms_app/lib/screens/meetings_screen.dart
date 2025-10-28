import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../services/api_service.dart';
import '../models/meeting_model.dart';

class MeetingsScreen extends StatefulWidget {
  const MeetingsScreen({super.key});

  @override
  State<MeetingsScreen> createState() => _MeetingsScreenState();
}

class _MeetingsScreenState extends State<MeetingsScreen> {
  List<Meeting> _meetings = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadMeetings();
  }

  Future<void> _loadMeetings() async {
    setState(() => _isLoading = true);
    try {
      final meetings = await ApiService.getUpcomingMeetings();
      setState(() => _meetings = meetings);
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
        onRefresh: _loadMeetings,
        child: _isLoading
            ? const Center(child: CircularProgressIndicator())
            : _meetings.isEmpty
                ? const Center(child: Text('No upcoming meetings'))
                : ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: _meetings.length,
                    itemBuilder: (context, index) {
                      final meeting = _meetings[index];
                      return Card(
                        margin: const EdgeInsets.only(bottom: 16),
                        child: ListTile(
                          leading: CircleAvatar(
                            backgroundColor: meeting.isToday ? Colors.green : Colors.blue,
                            child: Icon(
                              meeting.isToday ? Icons.today : Icons.event,
                              color: Colors.white,
                            ),
                          ),
                          title: Text(
                            meeting.title,
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                          subtitle: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              if (meeting.description != null)
                                Text(meeting.description!, maxLines: 2, overflow: TextOverflow.ellipsis),
                              const SizedBox(height: 4),
                              Text(
                                DateFormat('MMM d, y - HH:mm').format(meeting.meetingDate),
                                style: const TextStyle(fontSize: 12),
                              ),
                              Text(
                                'Duration: ${meeting.duration} min | Location: ${meeting.location ?? 'TBD'}',
                                style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                              ),
                            ],
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
