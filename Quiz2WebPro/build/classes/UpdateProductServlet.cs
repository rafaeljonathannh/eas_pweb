using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace WebEAS.Controllers
{
    public class ProductController : Controller
    {
        private readonly string _connectionString;

        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: /Product/UpdateProduct/{id}
        [HttpGet("Product/UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = "SELECT * FROM Products WHERE ProductId = @ProductId";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var product = new
                                {
                                    ProductId = reader["ProductId"],
                                    ProductName = reader["ProductName"],
                                    ProductQuantity = reader["ProductQuantity"],
                                    ProductPrice = reader["ProductPrice"]
                                };

                                return View(product);
                            }
                            else
                            {
                                TempData["Message"] = "Product not found!";
                                return RedirectToAction("Dashboard");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }

        // POST: /Product/UpdateProduct/{id}
        [HttpPost("Product/UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, string productName, int productQuantity, double productPrice)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(productName))
                {
                    TempData["Message"] = "Product name cannot be empty!";
                    return RedirectToAction("Dashboard");
                }
                if (productQuantity < 0)
                {
                    TempData["Message"] = "Product quantity cannot be negative!";
                    return RedirectToAction("Dashboard");
                }
                if (productPrice < 0)
                {
                    TempData["Message"] = "Product price cannot be negative!";
                    return RedirectToAction("Dashboard");
                }

                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = "UPDATE Products SET ProductName = @ProductName, ProductQuantity = @ProductQuantity, ProductPrice = @ProductPrice WHERE ProductId = @ProductId";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@ProductQuantity", productQuantity);
                        cmd.Parameters.AddWithValue("@ProductPrice", productPrice);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Product updated successfully!";
                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            TempData["Message"] = "Failed to update product!";
                            return RedirectToAction("Dashboard");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }
    }
}
