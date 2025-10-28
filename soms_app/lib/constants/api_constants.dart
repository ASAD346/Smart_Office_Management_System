class ApiConstants {
  // Base URL - Update this to your backend URL
  static const String baseUrl = 'http://192.168.18.42:5086/api'; // For Physical Device
  // static const String baseUrl = 'http://localhost:5086/api'; // For Web/Desktop
  // static const String baseUrl = 'http://10.0.2.2:5086/api'; // For Android Emulator
  
  // Auth endpoints
  static const String login = '/auth/login';
  
  // Employee endpoints
  static const String employees = '/employees';
  
  // Attendance endpoints
  static const String checkIn = '/attendance/checkin';
  static const String checkOut = '/attendance/checkout';
  static const String todayAttendance = '/attendance/today';
  static String attendanceByDate(String date) => '/attendance/date/$date';
  
  // Task endpoints
  static const String tasks = '/tasks';
  static const String myTasks = '/tasks/my-tasks';
  static String updateTaskStatus(int id) => '/tasks/$id/status';
  
  // Meeting endpoints
  static const String meetings = '/meetings';
  static const String myMeetings = '/meetings/my-meetings';
  static const String upcomingMeetings = '/meetings/upcoming';
}
