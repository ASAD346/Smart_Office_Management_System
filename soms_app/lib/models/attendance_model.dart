class Attendance {
  final int attendanceId;
  final int employeeId;
  final String employeeName;
  final DateTime checkInTime;
  final DateTime? checkOutTime;
  final String? checkInLocation;
  final String? checkOutLocation;
  final String status;
  final String? notes;

  Attendance({
    required this.attendanceId,
    required this.employeeId,
    required this.employeeName,
    required this.checkInTime,
    this.checkOutTime,
    this.checkInLocation,
    this.checkOutLocation,
    required this.status,
    this.notes,
  });

  factory Attendance.fromJson(Map<String, dynamic> json) {
    return Attendance(
      attendanceId: json['attendanceId'],
      employeeId: json['employeeId'],
      employeeName: json['employeeName'],
      checkInTime: DateTime.parse(json['checkInTime']),
      checkOutTime: json['checkOutTime'] != null 
          ? DateTime.parse(json['checkOutTime']) 
          : null,
      checkInLocation: json['checkInLocation'],
      checkOutLocation: json['checkOutLocation'],
      status: json['status'],
      notes: json['notes'],
    );
  }

  String get duration {
    if (checkOutTime == null) return 'Active';
    final diff = checkOutTime!.difference(checkInTime);
    final hours = diff.inHours;
    final minutes = diff.inMinutes % 60;
    return '${hours}h ${minutes}m';
  }
}
