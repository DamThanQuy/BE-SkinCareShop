using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test2.Models;

namespace test2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Test3Context _context;

        public UsersController(Test3Context context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/profile
        [HttpGet("profile")]
        public async Task<ActionResult<object>> GetCurrentUserProfile()
        {
            // Get user ID from claims (you need to implement authentication first)
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var user = await _context.Users
                .Where(u => u.UserId == int.Parse(userId))
                .Select(u => new {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Address,
                    UserType = u.Role,
                    u.FullName,
                    u.RegistrationDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/profile/{id}
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<object>> GetUserProfile(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Address,
                    UserType = u.Role,
                    u.FullName,
                    u.RegistrationDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateProfileRequest request)
        {
            // Get user ID from claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties if provided
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address;
            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Address,
                    UserType = user.Role,
                    user.FullName,
                    user.RegistrationDate
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the profile");
            }
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest registerRequest)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == registerRequest.Username);

            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }

            var newUser = new User
            {
                Name = registerRequest.Username,
                Password = registerRequest.Password // Lưu ý: Nên mã hóa mật khẩu trước khi lưu vào DB
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = newUser.UserId }, newUser);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _context.Users
                .Where(u => u.Name == loginRequest.Username && u.Password == loginRequest.Password)
                .Select(u => new {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Address,
                    UserType = u.Role,
                    u.FullName,
                    u.RegistrationDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Users/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserByAdmin(int id, [FromBody] UpdateUserRequest request)
        {
            // Verify if current user is admin
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || currentUserRole != "Admin")
            {
                return Unauthorized("Only administrators can update user information");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            // Update user properties if provided
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address;
            if (!string.IsNullOrEmpty(request.UserType))
                user.Role = request.UserType;
            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Address,
                    UserType = user.Role,
                    user.FullName,
                    user.RegistrationDate
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        // POST: api/Users/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                // For security reasons, don't reveal that the email doesn't exist
                return Ok(new { message = "If your email is registered, you will receive a password reset link." });
            }

            // Generate a random temporary password
            var tempPassword = GenerateRandomPassword();
            user.Password = tempPassword; // Note: In production, this should be hashed

            try
            {
                await _context.SaveChangesAsync();
                
                // Here you would typically send an email with the temporary password
                // For demo purposes, we'll just return it in the response
                return Ok(new { 
                    message = "Password has been reset. Please check your email.",
                    temporaryPassword = tempPassword // Remove this in production
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? UserType { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}
