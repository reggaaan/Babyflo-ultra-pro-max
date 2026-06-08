using Microsoft.AspNetCore.Mvc;
using BabyfloServer.Data;
using BabyfloServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace BabyfloServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        // FIX: Changed BabyfloContext to DataContext to match your project schema
        private readonly DataContext _context;
        
        public OrdersController(DataContext context) 
        { 
            _context = context; 
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Items)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Failed to load orders." });
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] ThesisCheckoutDto req)
        {
            if (req == null) return BadRequest(new { message = "Invalid checkout payload." });

            string calculatedStatus = "Pending Verification";
            if (!string.IsNullOrWhiteSpace(req.ReferenceNumber) && req.ReferenceNumber != "N/A (COD)")
            {
                calculatedStatus = "Paid / Confirmed";
            }

            try
            {
                var order = new Order
                {
                    CustomerName = req.CustomerName ?? string.Empty,
                    DeliveryAddress = req.DeliveryAddress ?? string.Empty,
                    TotalAmount = req.TotalAmount,
                    PaymentMethod = req.PaymentMethod ?? string.Empty,
                    ReferenceNumber = req.ReferenceNumber ?? string.Empty,
                    PaymentStatus = calculatedStatus,
                    OrderDate = DateTime.UtcNow
                };

                if (req.Items != null)
                {
                    foreach (var item in req.Items)
                    {
                        order.Items.Add(new OrderItem
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName ?? string.Empty,
                            Quantity = item.Quantity,
                            PriceAtPurchase = item.PriceAtPurchase
                        });
                    }
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order placed successfully!",
                    orderId = order.Id,
                    status = order.PaymentStatus
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Checkout processing failed.", details = ex.Message });
            }
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] ThesisCheckoutDto req)
        {
            if (req == null) return BadRequest("Payload is empty.");

            try
            {
                var order = new Order
                {
                    CustomerName = req.CustomerName ?? string.Empty,
                    DeliveryAddress = req.DeliveryAddress ?? string.Empty,
                    TotalAmount = req.TotalAmount,
                    PaymentMethod = req.PaymentMethod ?? string.Empty,
                    ReferenceNumber = req.ReferenceNumber ?? string.Empty,
                    PaymentStatus = "Pending Verification",
                    OrderDate = DateTime.UtcNow
                };

                if (req.Items != null)
                {
                    foreach (var item in req.Items)
                    {
                        order.Items.Add(new OrderItem
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName ?? string.Empty,
                            Quantity = item.Quantity,
                            PriceAtPurchase = item.PriceAtPurchase
                        });
                    }
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Server error during checkout.", details = ex.Message });
            }
        }
    }

    // 🎓 THESIS SAFEVIEW DTO: Captures everything sent by your Javascript checkout form 
    // without clashing with your strict database schemas or missing Enums!
    public class ThesisCheckoutDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public List<ThesisItemDto> Items { get; set; } = new List<ThesisItemDto>();
    }

    public class ThesisItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}