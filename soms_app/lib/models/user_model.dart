class User {
  final int userId;
  final String email;
  final String role;
  final int? employeeId;
  final String? fullName;
  final String token;

  User({
    required this.userId,
    required this.email,
    required this.role,
    this.employeeId,
    this.fullName,
    required this.token,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      userId: json['userId'],
      email: json['email'],
      role: json['role'],
      employeeId: json['employeeId'],
      fullName: json['fullName'],
      token: json['token'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'email': email,
      'role': role,
      'employeeId': employeeId,
      'fullName': fullName,
      'token': token,
    };
  }
}
