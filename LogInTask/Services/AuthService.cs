using LogInTask.Models;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;

namespace LogInTask.Services
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users = new()
        {
            new User { Username = "admin", Password = "$2b$12$Vr3R5erhyHSOP/Fqht/MGOEEWzv5wio3e8cMM5Qvuqdnbgp3jI0lq", Email = "admin@example.com", IsOtpEnabled = true, StaticOtp = "123456", IsActive = true },
            new User { Username = "user", Password = "$2b$12$lx1Mo27mqm2ZF3opO5PwveLX4ZVX8uAZE5qpqIE7XZYM7vrTIcMPe", Email = "user@example.com", IsOtpEnabled = false, StaticOtp = null, IsActive = true },
            new User { Username = "testuser", Password = "$2b$12$IgkhN2ukhUohX1I6O9/4Ge5fPDiNDAx1We2ksvnjDF85k9NMi8rrC", Email = "test@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = true },
            new User { Username = "iuser", Password = "$2b$12$lM4mdzxj//gmNGLmELREe.tv1ao0lr6oYbyD9URra7tMp0Tr.GLgq", Email = "iuser@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = false }
        };

        private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        public async Task<(bool success, string message)> LoginAsync(string identifier, string password, string otp = null)
        {
            await Task.Delay(200);

            User? user = null;
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

            if (!user.IsActive)
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