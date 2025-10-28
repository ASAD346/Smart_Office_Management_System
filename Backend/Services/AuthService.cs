using Microsoft.EntityFrameworkCore;
using SOMS.API.Data;
using SOMS.API.DTOs;
using SOMS.API.Helpers;

namespace SOMS.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !user.IsActive)
                return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            // Update last login
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user, user.Role!.RoleName);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role!.RoleName,
                EmployeeId = user.Employee?.EmployeeId,
                FullName = user.Employee?.FullName
            };
        }
    }
}
