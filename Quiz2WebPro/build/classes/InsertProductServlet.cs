using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace WebEAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string DB_URL = "Server=localhost;Database=Storify;User ID=root;Password=";

        [HttpPost("inputProduct")]
        public async Task<IActionResult> InputProduct([FromBody] ProductRequest request)
        {
            string name = request.ProductName;
            string quantity = request.ProductQuantity;
            string price = request.ProductPrice;

            using (var conn = new MySqlConnection(DB_URL))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO Products (productName, productQuantity, productPrice) VALUES (@ProductName, @ProductQuantity, @ProductPrice)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", name);
                        cmd.Parameters.AddWithValue("@ProductQuantity", quantity);
                        cmd.Parameters.AddWithValue("@ProductPrice", price);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product input success!" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Product input failed! Please try again." });
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

    public class ProductRequest
    {
        public string ProductName { get; set; }
        public string ProductQuantity { get; set; }
        public string ProductPrice { get; set; }
    }
}
