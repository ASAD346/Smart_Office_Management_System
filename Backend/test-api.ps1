# SOMS API Testing Script
# Run this after starting the API with 'dotnet run'

$baseUrl = "https://localhost:5001"

Write-Host "🔵 SOMS Backend API Test Suite" -ForegroundColor Cyan
Write-Host "================================`n" -ForegroundColor Cyan

# Disable SSL certificate validation for local testing
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint svcPoint, X509Certificate certificate,
            WebRequest webReq, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Test 1: Health Check
Write-Host "1️⃣  Testing API Health Check..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/test" -Method GET
    Write-Host "✅ API is running: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ API is not responding. Make sure 'dotnet run' is active." -ForegroundColor Red
    exit
}

# Test 2: Login
Write-Host "`n2️⃣  Testing Login..." -ForegroundColor Yellow
$loginData = @{
    email = "admin@soms.com"
    password = "admin123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✅ Login successful!" -ForegroundColor Green
    Write-Host "   User: $($loginResponse.fullName)" -ForegroundColor Gray
    Write-Host "   Role: $($loginResponse.role)" -ForegroundColor Gray
    Write-Host "   Token: $($token.Substring(0, 30))..." -ForegroundColor Gray
} catch {
    Write-Host "❌ Login failed. Check database and admin user." -ForegroundColor Red
    Write-Host "   Error: $_" -ForegroundColor Red
    exit
}

# Set up headers with token
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Test 3: Get Employees
Write-Host "`n3️⃣  Testing Get Employees..." -ForegroundColor Yellow
try {
    $employees = Invoke-RestMethod -Uri "$baseUrl/api/employees" -Method GET -Headers $headers
    Write-Host "✅ Retrieved $($employees.Count) employee(s)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No employees found or endpoint error" -ForegroundColor Red
}

# Test 4: Get Today's Attendance
Write-Host "`n4️⃣  Testing Get Today's Attendance..." -ForegroundColor Yellow
try {
    $attendance = Invoke-RestMethod -Uri "$baseUrl/api/attendance/today" -Method GET -Headers $headers
    Write-Host "✅ Retrieved $($attendance.Count) attendance record(s)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No attendance records or endpoint error" -ForegroundColor Red
}

# Test 5: Get All Meetings
Write-Host "`n5️⃣  Testing Get Meetings..." -ForegroundColor Yellow
try {
    $meetings = Invoke-RestMethod -Uri "$baseUrl/api/meetings" -Method GET -Headers $headers
    Write-Host "✅ Retrieved $($meetings.Count) meeting(s)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No meetings found or endpoint error" -ForegroundColor Red
}

# Test 6: Get All Tasks
Write-Host "`n6️⃣  Testing Get Tasks..." -ForegroundColor Yellow
try {
    $tasks = Invoke-RestMethod -Uri "$baseUrl/api/tasks" -Method GET -Headers $headers
    Write-Host "✅ Retrieved $($tasks.Count) task(s)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No tasks found or endpoint error" -ForegroundColor Red
}

# Test 7: Create a Meeting (Admin/HR only)
Write-Host "`n7️⃣  Testing Create Meeting..." -ForegroundColor Yellow
$meetingData = @{
    title = "Test Meeting"
    description = "API Test Meeting"
    meetingDate = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss")
    duration = 60
    location = "Conference Room A"
    attendeeIds = @(1)
} | ConvertTo-Json

try {
    $newMeeting = Invoke-RestMethod -Uri "$baseUrl/api/meetings" -Method POST -Headers $headers -Body $meetingData
    Write-Host "✅ Meeting created with ID: $($newMeeting.meetingId)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Failed to create meeting (check permissions)" -ForegroundColor Red
}

# Test 8: Create a Task (Admin/HR only)
Write-Host "`n8️⃣  Testing Create Task..." -ForegroundColor Yellow
$taskData = @{
    title = "Test Task"
    description = "API Test Task"
    assignedTo = 1
    priority = "Medium"
    dueDate = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")
} | ConvertTo-Json

try {
    $newTask = Invoke-RestMethod -Uri "$baseUrl/api/tasks" -Method POST -Headers $headers -Body $taskData
    Write-Host "✅ Task created with ID: $($newTask.taskId)" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Failed to create task (check permissions)" -ForegroundColor Red
}

# Test 9: Get Task Analytics
Write-Host "`n9️⃣  Testing Task Analytics..." -ForegroundColor Yellow
try {
    $analytics = Invoke-RestMethod -Uri "$baseUrl/api/reports/tasks" -Method GET -Headers $headers
    Write-Host "✅ Task Analytics Retrieved:" -ForegroundColor Green
    Write-Host "   Total Tasks: $($analytics.totalTasks)" -ForegroundColor Gray
    Write-Host "   Completed: $($analytics.completedTasks)" -ForegroundColor Gray
    Write-Host "   Completion Rate: $($analytics.completionRate)%" -ForegroundColor Gray
} catch {
    Write-Host "⚠️  Failed to get analytics" -ForegroundColor Red
}

# Summary
Write-Host "`n================================" -ForegroundColor Cyan
Write-Host "✅ API Testing Complete!" -ForegroundColor Green
Write-Host "`n📌 Your JWT Token (use for manual testing):" -ForegroundColor Cyan
Write-Host $token -ForegroundColor White
Write-Host "`n📖 See API_ENDPOINTS.md for full documentation" -ForegroundColor Cyan
