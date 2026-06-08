using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BabyfloServer.Data;
using BabyfloServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BabyfloServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            if (product == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/products/{id}/stock
        [HttpPatch("{id:int}/stock")]
        public async Task<IActionResult> ToggleStock(int id, [FromBody] StockUpdateDto data)
        {
            if (data == null) return BadRequest(new { message = "Missing body" });

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });

            product.InStock = data.InStock;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/products/{id}/best-seller  and /bestseller (support both)
        [HttpPatch("{id:int}/best-seller")]
        [HttpPatch("{id:int}/bestseller")]
        public async Task<ActionResult<Product>> UpdateBestSeller(int id, [FromBody] BestSellerUpdateDto data)
        {
            if (data == null) return BadRequest(new { message = "Missing body" });

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });

            product.IsBestSeller = data.IsBestSeller;
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // PATCH: api/products/{id}/discount
        [HttpPatch("{id:int}/discount")]
        public async Task<ActionResult<Product>> UpdateDiscount(int id, [FromBody] DiscountDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Missing body" });

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });

            product.Discount = dto.Discount;
            await _context.SaveChangesAsync();

            return Ok(product);
        }
    }

    public class StockUpdateDto { public bool InStock { get; set; } }

    public class BestSellerUpdateDto { public bool IsBestSeller { get; set; } }

    public class DiscountDto { public int Discount { get; set; } }

    public class ProductPatchDto
    {
        public decimal? Price { get; set; }
        public int? Discount { get; set; }
        public string? Description { get; set; }
    }
}