using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleERP.API.Data;
using SimpleERP.API.Models;

namespace SimpleERP.API.Controllers
{
    /// <summary>
    /// Order management API controller
    /// Handles customer orders and sales data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders with customer and product details
        /// GET /api/order
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                var result = new List<object>();

                foreach (var order in orders)
                {
                    // Manually get customer info by CustomerId
                    var customer = await _context.Users.FindAsync(order.CustomerId);
                    
                    result.Add(new 
                    {
                        order.Id,
                        order.OrderNumber,
                        CustomerName = customer?.Username ?? "Unknown",
                        CustomerEmail = customer?.Email ?? "Unknown",
                        order.TotalAmount,
                        order.Status,
                        order.OrderDate,
                        order.ShippedDate,
                        order.DeliveredDate,
                        order.ShippingAddress,
                        ItemCount = order.OrderItems.Count,
                        Items = order.OrderItems.Select(oi => new 
                        {
                            ProductName = oi.Product.Name,
                            oi.Quantity,
                            oi.UnitPrice,
                            oi.TotalPrice
                        })
                    });
                }

                return Ok(new 
                { 
                    message = "Orders retrieved successfully",
                    count = result.Count,
                    orders = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "Error retrieving orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Get orders summary and statistics
        /// GET /api/order/summary
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetOrdersSummary()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var totalOrders = await _context.Orders.CountAsync();
                var todayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
                var thisMonthOrders = await _context.Orders.CountAsync(o => o.OrderDate >= thisMonth);

                var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
                var todayRevenue = await _context.Orders
                    .Where(o => o.OrderDate.Date == today)
                    .SumAsync(o => o.TotalAmount);
                var thisMonthRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth)
                    .SumAsync(o => o.TotalAmount);

                var statusBreakdown = await _context.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new 
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync();

                var recentOrders = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync();

                var recentOrdersWithCustomers = new List<object>();
                foreach (var order in recentOrders)
                {
                    var customer = await _context.Users.FindAsync(order.CustomerId);
                    recentOrdersWithCustomers.Add(new 
                    {
                        order.Id,
                        order.OrderNumber,
                        CustomerName = customer?.Username ?? "Unknown",
                        order.TotalAmount,
                        order.Status,
                        order.OrderDate
                    });
                }

                return Ok(new 
                { 
                    message = "Orders summary retrieved successfully",
                    summary = new 
                    {
                        totalOrders = totalOrders,
                        todayOrders = todayOrders,
                        thisMonthOrders = thisMonthOrders,
                        totalRevenue = Math.Round(totalRevenue, 2),
                        todayRevenue = Math.Round(todayRevenue, 2),
                        thisMonthRevenue = Math.Round(thisMonthRevenue, 2)
                    },
                    statusBreakdown = statusBreakdown,
                    recentOrders = recentOrdersWithCustomers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders summary");
                return StatusCode(500, new { message = "Error retrieving orders summary", error = ex.Message });
            }
        }
    }
}