using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <returns>A success message.</returns>
        [HttpPost("register")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Register([FromBody] UserRegistrationModel model)
        {
            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Save the user to the database (not implemented)

            return Ok(new { Message = "User registered successfully" });
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="model">The user login model.</param>
        /// <returns>A JWT token.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel model)
        {
            // Verify the password (not implemented)
            string storedHashedPassword = "storedHashedPasswordFromDatabase"; // Replace with actual database retrieval
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, storedHashedPassword);

            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "User") // Replace with actual role retrieval
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
