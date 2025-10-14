using LogInTask.Models;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace LogInTask.Services
{
    public class AuthService : IAuthService
    {
        private  List<User> _users = new()
        {
            new User { Username = "admin", Password = "$2a$12$examplehashforadmin123", Email = "admin@example.com", IsOtpEnabled = true, StaticOtp = "123456", IsActive = true },  // Hash for "admin123"
            new User { Username = "user", Password = "$2a$12$examplehashforuser123", Email = "user@example.com", IsOtpEnabled = false, StaticOtp = null, IsActive = true },  // Hash for "user123"
            new User { Username = "testuser", Password = "$2a$12$examplehashfortest123", Email = "test@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = true },  // Hash for "test123"
            new User { Username = "iuser", Password = "$2a$12$examplehashfor123", Email = "iuser@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = false }  // Hash for "123"
        };

        private static Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        public async Task<(bool success, string message)> LoginAsync(string identifier, string password, string otp = null)
        {
            await Task.Delay(200);

            User? user;

            if (EmailRegex.IsMatch(identifier))  
            {
                user = GetUserByEmail(identifier);
            }
            else  
            {
                user = GetUserByUsername(identifier);
            }

            if (user == null)
                return (false, "❌ User not found.");
            if(!user.IsActive)
                return (false, "❌ User inactive.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return (false, "❌ Invalid password.");

            if (user.IsOtpEnabled)
            {
                if (string.IsNullOrEmpty(otp))
                    return (false, "OTP required");  
                if (otp != user.StaticOtp)
                    return (false, "❌ Invalid OTP.");
            }

            return (true, "✅ Login successful!");
        }

        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        }
        public User? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            
        }
    }
}