import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../constants/api_constants.dart';
import '../models/user_model.dart';
import '../models/attendance_model.dart';
import '../models/task_model.dart';
import '../models/meeting_model.dart';

class ApiService {
  static String? _token;

  // Get token from storage
  static Future<String?> getToken() async {
    if (_token != null) return _token;
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('token');
    return _token;
  }

  // Save token
  static Future<void> saveToken(String token) async {
    _token = token;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('token', token);
  }

  // Clear token
  static Future<void> clearToken() async {
    _token = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('token');
  }

  // Get headers
  static Future<Map<String, String>> getHeaders({bool includeAuth = true}) async {
    final headers = {
      'Content-Type': 'application/json',
    };

    if (includeAuth) {
      final token = await getToken();
      if (token != null) {
        headers['Authorization'] = 'Bearer $token';
      }
    }

    return headers;
  }

  // Generic API request method
  static Future<dynamic> request(
    String endpoint, {
    String method = 'GET',
    Map<String, dynamic>? body,
    bool includeAuth = true,
  }) async {
    final url = Uri.parse('${ApiConstants.baseUrl}$endpoint');
    final headers = await getHeaders(includeAuth: includeAuth);

    http.Response response;

    try {
      switch (method) {
        case 'POST':
          response = await http.post(url, headers: headers, body: jsonEncode(body));
          break;
        case 'PUT':
          response = await http.put(url, headers: headers, body: jsonEncode(body));
          break;
        case 'DELETE':
          response = await http.delete(url, headers: headers);
          break;
        default:
          response = await http.get(url, headers: headers);
      }

      if (response.statusCode == 200 || response.statusCode == 201) {
        if (response.body.isEmpty) return null;
        return jsonDecode(response.body);
      } else if (response.statusCode == 401) {
        await clearToken();
        throw Exception('Session expired. Please login again.');
      } else {
        final error = jsonDecode(response.body);
        throw Exception(error['message'] ?? 'An error occurred');
      }
    } catch (e) {
      rethrow;
    }
  }

  // Auth APIs
  static Future<User> login(String email, String password) async {
    final data = await request(
      ApiConstants.login,
      method: 'POST',
      body: {'email': email, 'password': password},
      includeAuth: false,
    );
    final user = User.fromJson(data);
    await saveToken(user.token);
    return user;
  }

  // Attendance APIs
  static Future<Attendance> checkIn(int employeeId, String location) async {
    final data = await request(
      ApiConstants.checkIn,
      method: 'POST',
      body: {'employeeId': employeeId, 'checkInLocation': location},
    );
    return Attendance.fromJson(data);
  }

  static Future<Attendance> checkOut(int employeeId, String location) async {
    final data = await request(
      ApiConstants.checkOut,
      method: 'POST',
      body: {'employeeId': employeeId, 'checkOutLocation': location},
    );
    return Attendance.fromJson(data);
  }

  static Future<List<Attendance>> getTodayAttendance() async {
    final data = await request(ApiConstants.todayAttendance);
    return (data as List).map((json) => Attendance.fromJson(json)).toList();
  }

  // Task APIs
  static Future<List<Task>> getMyTasks() async {
    final data = await request(ApiConstants.myTasks);
    return (data as List).map((json) => Task.fromJson(json)).toList();
  }

  static Future<void> updateTaskStatus(int taskId, String status) async {
    await request(
      ApiConstants.updateTaskStatus(taskId),
      method: 'PUT',
      body: {'status': status},
    );
  }

  // Meeting APIs
  static Future<List<Meeting>> getMyMeetings() async {
    final data = await request(ApiConstants.myMeetings);
    return (data as List).map((json) => Meeting.fromJson(json)).toList();
  }

  static Future<List<Meeting>> getUpcomingMeetings() async {
    final data = await request(ApiConstants.upcomingMeetings);
    return (data as List).map((json) => Meeting.fromJson(json)).toList();
  }
}
