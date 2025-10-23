using LogInTask.Models;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;

namespace LogInTask.Services
{
    public class AuthService : IAuthService
    {
        private  List<User> _users = new()
{
    new User { Username = "john",    Email = "john@example.com", Password = BCrypt.Net.BCrypt.HashPassword("123"),  IsOtpEnabled = false, StaticOtp = null,     IsActive = true },
    new User { Username = "jane",    Email = "jane@example.com", Password = BCrypt.Net.BCrypt.HashPassword("456"),  IsOtpEnabled = true,  StaticOtp = "000000", IsActive = true },
    new User { Username = "inactive",Email = "inactive@example.com", Password = BCrypt.Net.BCrypt.HashPassword("000"),  IsOtpEnabled = false, StaticOtp = null,     IsActive = false }
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