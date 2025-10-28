-- SOMS: Create Admin User
-- Run this script AFTER starting the API and getting the password hash

-- Step 1: Get the BCrypt hash for 'admin123'
-- Navigate to: https://localhost:5001/api/auth/generate-hash/admin123
-- Copy the hash value from the response

-- Step 2: Replace 'YOUR_BCRYPT_HASH_HERE' below with the actual hash
-- Step 3: Run this script in MySQL

USE soms_db;

-- Insert admin user
INSERT INTO Users (Email, PasswordHash, RoleId, IsActive, CreatedAt) 
VALUES ('admin@soms.com', 'YOUR_BCRYPT_HASH_HERE', 1, TRUE, NOW());

-- Get the created UserId
SET @userId = LAST_INSERT_ID();

-- Insert employee record
INSERT INTO Employees (UserId, FirstName, LastName, PhoneNumber, DepartmentId, Position, JoinDate) 
VALUES (@userId, 'Admin', 'User', '1234567890', 1, 'System Administrator', '2024-01-01');

-- Verify
SELECT 
    u.UserId, 
    u.Email, 
    r.RoleName,
    e.FirstName,
    e.LastName,
    e.Position
FROM Users u
JOIN Roles r ON u.RoleId = r.RoleId
LEFT JOIN Employees e ON u.UserId = e.UserId
WHERE u.Email = 'admin@soms.com';
