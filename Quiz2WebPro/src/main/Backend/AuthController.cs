using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace WebEAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string DB_URL = "Server=localhost;Database=storify;User ID=root;Password=;";

        // POST: api/Auth/updateProfile
        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var loggedUser = HttpContext.Session.GetString("loggedUser");
            if (string.IsNullOrEmpty(loggedUser))
            {
                return Unauthorized(new { message = "You are not authorized to update this profile." });
            }

            try
            {
                using var conn = new MySqlConnection(DB_URL);
                await conn.OpenAsync();

                string query = @"UPDATE users SET UserFullName = @FullName, 
                                UserEmail = @Email, UserAddress = @Address 
                                WHERE UserName = @UserName";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", request.FullName);
                cmd.Parameters.AddWithValue("@Email", request.Email);
                cmd.Parameters.AddWithValue("@Address", request.Address);
                cmd.Parameters.AddWithValue("@UserName", loggedUser);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Profile updated successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to update profile. Try again later." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error occurred: {ex.Message}" });
            }
        }
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
