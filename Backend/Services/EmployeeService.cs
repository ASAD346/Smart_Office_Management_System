using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;
using SOMS.API.Models;

namespace SOMS.API.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.User)
                .ThenInclude(u => u.Role)
                .Include(e => e.Department)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    UserId = e.UserId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = e.FullName,
                    PhoneNumber = e.PhoneNumber,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                    Position = e.Position,
                    JoinDate = e.JoinDate,
                    ProfilePicture = e.ProfilePicture,
                    Address = e.Address,
                    Email = e.User!.Email,
                    Role = e.User.Role!.RoleName,
                    IsActive = e.User.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .ThenInclude(u => u.Role)
                .Include(e => e.Department)
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    UserId = e.UserId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = e.FullName,
                    PhoneNumber = e.PhoneNumber,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                    Position = e.Position,
                    JoinDate = e.JoinDate,
                    ProfilePicture = e.ProfilePicture,
                    Address = e.Address,
                    Email = e.User!.Email,
                    Role = e.User.Role!.RoleName,
                    IsActive = e.User.IsActive
                })
                .FirstOrDefaultAsync();

            return employee;
        }

        public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            // Create user
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create employee
            var employee = new Employee
            {
                UserId = user.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                DepartmentId = dto.DepartmentId,
                Position = dto.Position,
                JoinDate = dto.JoinDate,
                Address = dto.Address
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await GetEmployeeByIdAsync(employee.EmployeeId);
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return null;

            // Update employee details
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.DepartmentId = dto.DepartmentId;
            employee.Position = dto.Position;
            employee.Address = dto.Address;

            // Update user status if provided
            if (dto.IsActive.HasValue && employee.User != null)
            {
                employee.User.IsActive = dto.IsActive.Value;
            }

            await _context.SaveChangesAsync();

            return await GetEmployeeByIdAsync(id);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return false;

            // Delete user (cascade will delete employee)
            if (employee.User != null)
            {
                _context.Users.Remove(employee.User);
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
