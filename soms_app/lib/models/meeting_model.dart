class Meeting {
  final int meetingId;
  final String title;
  final String? description;
  final DateTime meetingDate;
  final int duration;
  final String? location;
  final int createdBy;
  final String createdByName;
  final DateTime createdAt;

  Meeting({
    required this.meetingId,
    required this.title,
    this.description,
    required this.meetingDate,
    required this.duration,
    this.location,
    required this.createdBy,
    required this.createdByName,
    required this.createdAt,
  });

  factory Meeting.fromJson(Map<String, dynamic> json) {
    return Meeting(
      meetingId: json['meetingId'],
      title: json['title'],
      description: json['description'],
      meetingDate: DateTime.parse(json['meetingDate']),
      duration: json['duration'],
      location: json['location'],
      createdBy: json['createdBy'],
      createdByName: json['createdByName'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }

  bool get isUpcoming {
    return meetingDate.isAfter(DateTime.now());
  }

  bool get isToday {
    final now = DateTime.now();
    return meetingDate.year == now.year &&
        meetingDate.month == now.month &&
        meetingDate.day == now.day;
  }
}
