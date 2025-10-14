using LogInTask.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LogInTask.Services
{
    public class AuthService : IAuthService
    {
        private  List<User> _users = new()
        {
            new User { Username = "admin", Password = "admin123", Email = "admin@example.com", IsOtpEnabled = true, StaticOtp = "123456", IsActive = true },
            new User { Username = "user", Password = "user123", Email = "user@example.com", IsOtpEnabled = false, StaticOtp = null, IsActive = true },
            new User { Username = "testuser", Password = "test123", Email = "test@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = true },
            new User { Username = "iuser", Password = "123", Email = "iuser@example.com", IsOtpEnabled = true, StaticOtp = "654321", IsActive = false }

        };

        public async Task<(bool success, string message)> LoginAsync(string username, string password, string otp = null)
        {
            await Task.Delay(200);

            var user = GetUserByUsername(username);

            if (user == null)
                return (false, "❌ User not found.");
            if(!user.IsActive)
                return (false, "❌ User inactive.");

            if (user.Password != password)
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

        public User GetUserByUsername(string username)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            return user ?? throw new InvalidOperationException("User not found.");
        }
    }
}