using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebEAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string DB_URL = "Server=localhost;Database=Storify;User ID=root;Password=";

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string username = request.Username;
            string password = request.Password;

            using (var conn = new MySqlConnection(DB_URL))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT * FROM Users WHERE UserName = @UserName AND UserPassword = @UserPassword";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@UserPassword", password);

                        var reader = await cmd.ExecuteReaderAsync();
                        
                        if (await reader.ReadAsync())
                        {
                            HttpContext.Session.SetString("loggedUser", username);

                            return Ok(new { message = "Login successful", redirectUrl = "/dashboard" });
                        }
                        else
                        {
                            return Unauthorized(new { message = "Invalid username or password. Try again." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error occurred: {ex.Message}" });
                }
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
