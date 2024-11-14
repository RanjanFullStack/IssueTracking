using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using IssueTracking.Models; // Assuming ApplicationDbContext is in this namespace
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <returns>A success message.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            // Check if any users exist in the database
            bool isFirstUser = !_context.Users.Any();

            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create the new user with role
            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = hashedPassword,
                Role = isFirstUser ? "Admin" : "User" // Assign Admin role to the first user
            };

            // Save the user to the database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = isFirstUser ? "Admin user registered successfully" : "User registered successfully" });
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="model">The user login model.</param>
        /// <returns>A JWT token.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            // Retrieve user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Verify the password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Generate JWT token with role
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class UserRegistrationModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
