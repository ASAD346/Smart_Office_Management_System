# SOMS Backend API

## ✅ Setup Complete!

The ASP.NET Core Web API backend is now configured with:
- ✅ MySQL database connection (Entity Framework Core)
- ✅ JWT authentication
- ✅ All database models (User, Employee, Attendance, Meeting, Task, etc.)
- ✅ Auth controller with login endpoint
- ✅ Swagger/OpenAPI documentation
- ✅ CORS enabled for frontend communication

---

## 📋 Next Steps

### 1. **Configure MySQL Password**
Edit `appsettings.json` and replace `YOUR_PASSWORD_HERE` with your MySQL root password:

```json
"DefaultConnection": "Server=localhost;Port=3306;Database=soms_db;User=root;Password=YOUR_ACTUAL_PASSWORD;"
```

### 2. **Create Test User in Database**
Run this SQL in MySQL to create an admin user:

```sql
USE soms_db;

-- Insert admin user (password: admin123)
INSERT INTO Users (Email, PasswordHash, RoleId, IsActive, CreatedAt) 
VALUES ('admin@soms.com', '$2a$11$XyZ9abcDEF123456789012u.HASH_PLACEHOLDER', 1, TRUE, NOW());

-- Get the UserId
SET @userId = LAST_INSERT_ID();

-- Insert employee record
INSERT INTO Employees (UserId, FirstName, LastName, PhoneNumber, DepartmentId, Position, JoinDate) 
VALUES (@userId, 'Admin', 'User', '1234567890', 1, 'System Administrator', '2024-01-01');
```

**Note**: The password hash shown is a placeholder. You'll need to generate a proper BCrypt hash. You can do this by:
- Running the API and using a registration endpoint (we can create one)
- Using an online BCrypt generator for "admin123"

### 3. **Run the API**

```bash
dotnet run
```

The API will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### 4. **Test the API**

#### Test endpoint:
```bash
GET https://localhost:5001/api/auth/test
```

#### Login endpoint:
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@soms.com",
  "password": "admin123"
}
```

---

## 🗂️ Project Structure

```
Backend/
├── Controllers/
│   └── AuthController.cs          # Login endpoint
├── Models/                         # Database entities
│   ├── User.cs
│   ├── Role.cs
│   ├── Employee.cs
│   ├── Department.cs
│   ├── Attendance.cs
│   ├── Meeting.cs
│   ├── MeetingAttendee.cs
│   └── Task.cs
├── DTOs/                          # Data transfer objects
│   ├── LoginRequestDto.cs
│   └── LoginResponseDto.cs
├── Services/
│   └── AuthService.cs             # Authentication logic
├── Data/
│   └── ApplicationDbContext.cs    # EF Core context
├── Helpers/
│   └── JwtHelper.cs               # JWT token generation
├── Program.cs                     # App configuration
└── appsettings.json              # Configuration settings
```

---

## 🔧 What We'll Build Next

### Phase 1: Employee Management API
- `GET /api/employees` - List all employees
- `GET /api/employees/{id}` - Get employee details
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Phase 2: Attendance API
- `POST /api/attendance/checkin` - Mark check-in
- `POST /api/attendance/checkout` - Mark check-out
- `GET /api/attendance/today` - Today's attendance
- `GET /api/attendance/report` - Attendance reports

### Phase 3: Meeting API
- `GET /api/meetings` - List meetings
- `POST /api/meetings` - Schedule meeting
- `PUT /api/meetings/{id}` - Update meeting
- `DELETE /api/meetings/{id}` - Cancel meeting

### Phase 4: Task API
- `GET /api/tasks` - List tasks
- `POST /api/tasks` - Create task
- `PUT /api/tasks/{id}` - Update task status
- `DELETE /api/tasks/{id}` - Delete task

---

## 🚀 Quick Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Watch mode (auto-reload on changes)
dotnet watch run

# Create migration (after model changes)
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

---

## 📝 Notes

- The API uses **BCrypt** for password hashing
- **JWT tokens** expire after 24 hours (configurable in appsettings.json)
- **CORS** is enabled to allow frontend access
- **Swagger UI** is available in development mode for API testing
