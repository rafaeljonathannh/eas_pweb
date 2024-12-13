using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

namespace WebEAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string DB_URL = "Server=localhost;Database=Storify;User ID=root;Password=";

        [HttpPost("deleteProduct")]
        public IActionResult DeleteProduct([FromBody] ProductRequest request)
        {
            string productId = request.ProductID;

            using (var conn = new MySqlConnection(DB_URL))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product has been deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = "Product not found." });
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    return StatusCode(500, new { message = $"Database error: {ex.Message}" });
                }
            }
        }
    }

    public class ProductRequest
    {
        public string ProductID { get; set; }
    }
}
