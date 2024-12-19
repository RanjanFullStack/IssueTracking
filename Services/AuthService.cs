using BCrypt.Net;
using IssueTracking.Services;

namespace IssueTracking.Services
{
    public class AuthService : IAuthService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
