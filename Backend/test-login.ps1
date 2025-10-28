# Test SOMS API Login

Write-Host "Testing SOMS API Login..." -ForegroundColor Cyan

$body = @{
    email = "admin@soms.com"
    password = "admin123"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5086/api/auth/login" -Method POST -Body $body -ContentType "application/json"
    
    Write-Host "`n✅ LOGIN SUCCESSFUL!" -ForegroundColor Green
    Write-Host "`nResponse:" -ForegroundColor Yellow
    Write-Host "Token: $($response.token.Substring(0,50))..." -ForegroundColor White
    Write-Host "User ID: $($response.userId)" -ForegroundColor White
    Write-Host "Email: $($response.email)" -ForegroundColor White
    Write-Host "Role: $($response.role)" -ForegroundColor White
    Write-Host "Employee ID: $($response.employeeId)" -ForegroundColor White
    Write-Host "Full Name: $($response.fullName)" -ForegroundColor White
}
catch {
    Write-Host "`n❌ LOGIN FAILED!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}
