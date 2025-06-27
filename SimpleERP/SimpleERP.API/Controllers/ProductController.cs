using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleERP.API.Data;
using SimpleERP.API.Models;

namespace SimpleERP.API.Controllers
{
    /// <summary>
    /// Product management API controller
    /// Handles inventory and product operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with stock information
        /// GET /api/product
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .Select(p => new 
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.Category,
                        p.SKU,
                        p.CreatedAt,
                        p.UpdatedAt,
                        StockStatus = p.StockQuantity > 10 ? "In Stock" : 
                                     p.StockQuantity > 0 ? "Low Stock" : "Out of Stock"
                    })
                    .ToListAsync();

                return Ok(new 
                { 
                    message = "Products retrieved successfully",
                    count = products.Count,
                    products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new { message = "Error retrieving products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get product by ID
        /// GET /api/product/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Where(p => p.Id == id && p.IsActive)
                    .Select(p => new 
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.Category,
                        p.SKU,
                        p.CreatedAt,
                        p.UpdatedAt,
                        StockStatus = p.StockQuantity > 10 ? "In Stock" : 
                                     p.StockQuantity > 0 ? "Low Stock" : "Out of Stock"
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(new 
                { 
                    message = "Product retrieved successfully",
                    product = product
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product {id}");
                return StatusCode(500, new { message = "Error retrieving product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory summary
        /// GET /api/product/inventory
        /// </summary>
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventorySummary()
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                var totalStockValue = await _context.Products
                    .Where(p => p.IsActive)
                    .SumAsync(p => p.Price * p.StockQuantity);
                
                var lowStockProducts = await _context.Products
                    .Where(p => p.IsActive && p.StockQuantity <= 10 && p.StockQuantity > 0)
                    .CountAsync();
                
                var outOfStockProducts = await _context.Products
                    .Where(p => p.IsActive && p.StockQuantity == 0)
                    .CountAsync();

                var categoryBreakdown = await _context.Products
                    .Where(p => p.IsActive)
                    .GroupBy(p => p.Category)
                    .Select(g => new 
                    {
                        Category = g.Key,
                        ProductCount = g.Count(),
                        TotalValue = g.Sum(p => p.Price * p.StockQuantity),
                        TotalStock = g.Sum(p => p.StockQuantity)
                    })
                    .ToListAsync();

                return Ok(new 
                { 
                    message = "Inventory summary retrieved successfully",
                    summary = new 
                    {
                        totalProducts = totalProducts,
                        totalStockValue = Math.Round(totalStockValue, 2),
                        lowStockProducts = lowStockProducts,
                        outOfStockProducts = outOfStockProducts,
                        inStockProducts = totalProducts - lowStockProducts - outOfStockProducts
                    },
                    categoryBreakdown = categoryBreakdown
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory summary");
                return StatusCode(500, new { message = "Error retrieving inventory summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Search products by name or category
        /// GET /api/product/search?query=laptop
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query is required" });
                }

                var products = await _context.Products
                    .Where(p => p.IsActive && 
                               (p.Name.Contains(query) || 
                                p.Description.Contains(query) || 
                                p.Category.Contains(query) ||
                                p.SKU.Contains(query)))
                    .Select(p => new 
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.Category,
                        p.SKU,
                        StockStatus = p.StockQuantity > 10 ? "In Stock" : 
                                     p.StockQuantity > 0 ? "Low Stock" : "Out of Stock"
                    })
                    .ToListAsync();

                return Ok(new 
                { 
                    message = $"Search completed for '{query}'",
                    count = products.Count,
                    products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching products with query: {query}");
                return StatusCode(500, new { message = "Error searching products", error = ex.Message });
            }
        }
    }
}