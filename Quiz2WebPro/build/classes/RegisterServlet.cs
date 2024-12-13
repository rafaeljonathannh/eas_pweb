using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace WebEAS.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string userName, string fullName, string email, string address, string password, string dobDay, string dobMonth, string dobYear)
        {
            string dateOfBirth = $"{dobYear}-{dobMonth}-{dobDay}";

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = "INSERT INTO Users (UserEmail, UserName, UserPassword, UserDOB, UserFullName, UserAddress) VALUES (@UserEmail, @UserName, @UserPassword, @UserDOB, @UserFullName, @UserAddress)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserEmail", email);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@UserPassword", password);
                        cmd.Parameters.AddWithValue("@UserDOB", dateOfBirth);
                        cmd.Parameters.AddWithValue("@UserFullName", fullName);
                        cmd.Parameters.AddWithValue("@UserAddress", address);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Registration successful! You can now log in.";
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            TempData["Message"] = "Registration failed! Please try again.";
                            return RedirectToAction("Register");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error occurred: {ex.Message}";
                return RedirectToAction("Register");
            }
        }
    }
}
