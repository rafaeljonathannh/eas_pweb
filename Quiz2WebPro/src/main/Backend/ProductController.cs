using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

public class ProductController : Controller
{
    private readonly string connectionString = "Server=localhost;Database=storify;User Id=root;Password=;";
    
    public IActionResult Index()
    {
        List<Product> products = new List<Product>();
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM products";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = (int)reader["ProductId"],
                            ProductName = reader["ProductName"].ToString(),
                            Quantity = (int)reader["ProductQuantity"],
                            Price = (decimal)reader["ProductPrice"]
                        });
                    }
                }
            }
        }
        catch
        {
            ViewData["Error"] = "Error retrieving products.";
        }

        return View(products);
    }

    [HttpPost]
    public IActionResult Delete(int productId)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch
        {
            ViewData["Error"] = "Error deleting product.";
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Update(int productId)
    {
        return RedirectToAction("Edit", "Product", new { id = productId });
    }
}
