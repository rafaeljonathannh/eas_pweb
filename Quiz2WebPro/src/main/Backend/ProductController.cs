using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductController : Controller
{
    private readonly string connectionString = "Server=localhost;Database=storify;User Id=root;Password=;";

    // GET: /Product/InputProduct - Renders Add Product form
    [HttpGet("/Product/InputProduct")]
    public IActionResult InputProduct()
    {
        return View();
    }

    // POST: /api/Product/inputProduct - Inserts a new product
    [HttpPost("inputProduct")]
    public async Task<IActionResult> InputProduct([FromBody] ProductRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProductName) || request.ProductQuantity < 0 || request.ProductPrice < 0)
                return BadRequest(new { message = "Invalid input data." });

            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = "INSERT INTO Products (ProductName, ProductQuantity, ProductPrice) VALUES (@ProductName, @ProductQuantity, @ProductPrice)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductName", request.ProductName);
                    cmd.Parameters.AddWithValue("@ProductQuantity", request.ProductQuantity);
                    cmd.Parameters.AddWithValue("@ProductPrice", request.ProductPrice);

                    await cmd.ExecuteNonQueryAsync();
                    return Ok(new { message = "Product added successfully!" });
                }
            }
        }
        catch
        {
            return StatusCode(500, new { message = "Error adding product." });
        }
    }

    // GET: /Product/UpdateProduct/{id} - Renders Update Product form with data
    [HttpGet("/Product/UpdateProduct/{id}")]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        Product product = null;

        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Product
                            {
                                ProductId = (int)reader["ProductId"],
                                ProductName = reader["ProductName"].ToString(),
                                Quantity = (int)reader["ProductQuantity"],
                                Price = (decimal)reader["ProductPrice"]
                            };
                        }
                    }
                }
            }
        }
        catch
        {
            TempData["Error"] = "Error loading product.";
        }

        if (product == null)
            return RedirectToAction("Index");

        return View(product);
    }

    // POST: /api/Product/UpdateProduct - Updates an existing product
    [HttpPost("UpdateProduct")]
    public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateRequest request)
    {
        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = @"UPDATE Products 
                                 SET ProductName = @ProductName, 
                                     ProductQuantity = @ProductQuantity, 
                                     ProductPrice = @ProductPrice 
                                 WHERE ProductId = @ProductId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", request.ProductId);
                    cmd.Parameters.AddWithValue("@ProductName", request.ProductName);
                    cmd.Parameters.AddWithValue("@ProductQuantity", request.ProductQuantity);
                    cmd.Parameters.AddWithValue("@ProductPrice", request.ProductPrice);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                        return Ok(new { message = "Product updated successfully!" });
                    else
                        return BadRequest(new { message = "Failed to update product." });
                }
            }
        }
        catch
        {
            return StatusCode(500, new { message = "Error updating product." });
        }
    }
}

// Request Models
public class ProductRequest
{
    public string ProductName { get; set; }
    public int ProductQuantity { get; set; }
    public decimal ProductPrice { get; set; }
}

public class ProductUpdateRequest : ProductRequest
{
    public int ProductId { get; set; }
}

// Product Model
public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
