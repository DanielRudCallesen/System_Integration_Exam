using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, InStock = true },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, InStock = true },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, InStock = false }
        };

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "ProductService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { error = $"Product with ID {id} not found" });
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { error = "Product name is required" });
            }

            if (request.Price <= 0)
            {
                return BadRequest(new { error = "Price must be greater than 0" });
            }

            var product = new Product
            {
                Id = Products.Max(p => p.Id) + 1,
                Name = request.Name,
                Price = request.Price,
                InStock = request.InStock
            };

            Products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateProductRequest request)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { error = $"Product with ID {id} not found" });
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                product.Name = request.Name;
            }

            if (request.Price.HasValue)
            {
                if (request.Price.Value <= 0)
                {
                    return BadRequest(new { error = "Price must be greater than 0" });
                }
                product.Price = request.Price.Value;
            }

            if (request.InStock.HasValue)
            {
                product.InStock = request.InStock.Value;
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { error = $"Product with ID {id} not found" });
            }

            Products.Remove(product);
            return NoContent();
        }
    }
}

public record Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool InStock { get; set; }
}

public record CreateProductRequest(string Name, decimal Price, bool InStock);

public record UpdateProductRequest(string? Name, decimal? Price, bool? InStock);