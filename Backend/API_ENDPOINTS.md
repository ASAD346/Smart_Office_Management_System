# SOMS Backend API Endpoints

## 🔐 Authentication

All endpoints (except `/api/auth/login`) require a JWT token in the Authorization header:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## 📋 Table of Contents
1. [Authentication](#authentication)
2. [Employees](#employees)
3. [Attendance](#attendance)
4. [Meetings](#meetings)
5. [Tasks](#tasks)
6. [Reports](#reports)

---

## 1. Authentication

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@soms.com",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "email": "admin@soms.com",
  "role": "Admin",
  "employeeId": 1,
  "fullName": "Admin User"
}
```

### Test Endpoint
```http
GET /api/auth/test
```

### Generate Password Hash
```http
GET /api/auth/generate-hash/{password}
```

---

## 2. Employees

### Get All Employees
```http
GET /api/employees
Authorization: Bearer {token}
```

### Get Employee by ID
```http
GET /api/employees/{id}
Authorization: Bearer {token}
```

### Create Employee (Admin/HR only)
```http
POST /api/employees
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "john.doe@soms.com",
  "password": "password123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "1234567890",
  "departmentId": 1,
  "position": "Software Developer",
  "joinDate": "2024-01-15",
  "roleId": 3
}
```

### Update Employee (Admin/HR only)
```http
PUT /api/employees/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "9876543210",
  "departmentId": 2,
  "position": "Senior Developer"
}
```

### Delete Employee (Admin only)
```http
DELETE /api/employees/{id}
Authorization: Bearer {token}
```

---

## 3. Attendance

### Check In
```http
POST /api/attendance/checkin
Authorization: Bearer {token}
Content-Type: application/json

{
  "employeeId": 1,
  "checkInLocation": "Office - Building A"
}
```

### Check Out
```http
POST /api/attendance/checkout
Authorization: Bearer {token}
Content-Type: application/json

{
  "employeeId": 1,
  "checkOutLocation": "Office - Building A"
}
```

### Get Today's Attendance
```http
GET /api/attendance/today
Authorization: Bearer {token}
```

### Get Attendance by Date
```http
GET /api/attendance/date/2024-01-15
Authorization: Bearer {token}
```

### Get Employee Attendance
```http
GET /api/attendance/employee/{employeeId}?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```

### Get Attendance by ID
```http
GET /api/attendance/{id}
Authorization: Bearer {token}
```

---

## 4. Meetings

### Get All Meetings
```http
GET /api/meetings
Authorization: Bearer {token}
```

### Get My Meetings
```http
GET /api/meetings/my-meetings
Authorization: Bearer {token}
```

### Get Upcoming Meetings
```http
GET /api/meetings/upcoming
Authorization: Bearer {token}
```

### Get Meeting by ID
```http
GET /api/meetings/{id}
Authorization: Bearer {token}
```

### Create Meeting (Admin/HR only)
```http
POST /api/meetings
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Project Planning Meeting",
  "description": "Q1 2024 project planning and resource allocation",
  "meetingDate": "2024-02-15T10:00:00",
  "duration": 60,
  "location": "Conference Room A",
  "attendeeIds": [1, 2, 3, 4]
}
```

### Update Meeting (Admin/HR only)
```http
PUT /api/meetings/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated Meeting Title",
  "meetingDate": "2024-02-15T14:00:00",
  "duration": 90,
  "attendeeIds": [1, 2, 3, 4, 5]
}
```

### Delete Meeting (Admin/HR only)
```http
DELETE /api/meetings/{id}
Authorization: Bearer {token}
```

### Update Attendee Status
```http
PUT /api/meetings/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Accepted"
}
```
*Status options: Accepted, Declined, Attended*

---

## 5. Tasks

### Get All Tasks
```http
GET /api/tasks
Authorization: Bearer {token}
```

### Filter Tasks
```http
GET /api/tasks?employeeId=1&status=Pending
Authorization: Bearer {token}
```

### Get My Tasks
```http
GET /api/tasks/my-tasks
Authorization: Bearer {token}
```

### Get Overdue Tasks
```http
GET /api/tasks/overdue
Authorization: Bearer {token}
```

### Get Task by ID
```http
GET /api/tasks/{id}
Authorization: Bearer {token}
```

### Create Task (Admin/HR only)
```http
POST /api/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Implement Login Feature",
  "description": "Develop the user authentication system with JWT",
  "assignedTo": 2,
  "priority": "High",
  "startDate": "2024-01-20",
  "dueDate": "2024-01-30"
}
```
*Priority options: Low, Medium, High, Urgent*

### Update Task
```http
PUT /api/tasks/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated Task Title",
  "status": "In Progress",
  "priority": "Urgent"
}
```
*Status options: Pending, In Progress, Completed, Cancelled*

### Update Task Status
```http
PUT /api/tasks/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Completed"
}
```

### Delete Task (Admin/HR only)
```http
DELETE /api/tasks/{id}
Authorization: Bearer {token}
```

---

## 6. Reports (Admin/HR only)

### Get Attendance Report
```http
GET /api/reports/attendance?startDate=2024-01-01&endDate=2024-01-31&departmentId=1
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "employeeId": 1,
    "employeeName": "John Doe",
    "department": "IT",
    "totalDays": 31,
    "presentDays": 28,
    "lateDays": 2,
    "absentDays": 3,
    "halfDays": 1,
    "attendancePercentage": 90.32,
    "totalHoursWorked": 224.5
  }
]
```

### Get Task Analytics
```http
GET /api/reports/tasks?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```

**Response:**
```json
{
  "totalTasks": 50,
  "pendingTasks": 10,
  "inProgressTasks": 15,
  "completedTasks": 23,
  "cancelledTasks": 2,
  "completionRate": 46.0,
  "overdueTasks": 5,
  "tasksByEmployee": [
    {
      "employeeId": 2,
      "employeeName": "John Doe",
      "assignedTasks": 12,
      "completedTasks": 10,
      "completionRate": 83.33
    }
  ]
}
```

### Get Daily Attendance
```http
GET /api/reports/daily-attendance?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```

### Get Employee Performance
```http
GET /api/reports/employee-performance?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "employeeId": 2,
    "employeeName": "John Doe",
    "department": "IT",
    "attendanceRate": 93.55,
    "tasksCompleted": 10,
    "tasksAssigned": 12,
    "taskCompletionRate": 83.33,
    "meetingsAttended": 15
  }
]
```

### Export Attendance Report (CSV)
```http
GET /api/reports/attendance/export?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```
Downloads: `AttendanceReport_20240101_20240131.csv`

### Export Task Analytics (CSV)
```http
GET /api/reports/tasks/export?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer {token}
```
Downloads: `TaskAnalytics_20240120143055.csv`

---

## 🔑 Role-Based Access Control

| Endpoint | Admin | HR | Employee |
|----------|-------|----|---------| 
| **Authentication** | ✅ | ✅ | ✅ |
| **View Employees** | ✅ | ✅ | ✅ |
| **Create/Update Employee** | ✅ | ✅ | ❌ |
| **Delete Employee** | ✅ | ❌ | ❌ |
| **Attendance (Self)** | ✅ | ✅ | ✅ |
| **View All Attendance** | ✅ | ✅ | ❌ |
| **View Meetings** | ✅ | ✅ | ✅ |
| **Create/Update/Delete Meetings** | ✅ | ✅ | ❌ |
| **Update Meeting Status (Self)** | ✅ | ✅ | ✅ |
| **View Tasks** | ✅ | ✅ | ✅ |
| **Create/Delete Tasks** | ✅ | ✅ | ❌ |
| **Update Task Status** | ✅ | ✅ | ✅ |
| **Reports & Analytics** | ✅ | ✅ | ❌ |

---

## 🚀 Testing with PowerShell

### Set Token Variable
```powershell
$token = "YOUR_JWT_TOKEN_HERE"
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}
```

### Test Login
```powershell
$loginData = @{
    email = "admin@soms.com"
    password = "admin123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $response.token
```

### Test Get Employees
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/employees" -Method GET -Headers $headers
```

---

## 📊 Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## 🛠️ API Base URL

- **Development (HTTPS):** `https://localhost:5001`
- **Development (HTTP):** `http://localhost:5000`
