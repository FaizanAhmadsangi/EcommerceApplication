using EcommerceApplication.API.Commons;
using EcommerceApplication_BAL.Services;
using EcommerceApplication_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Add this namespace
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ServiceProduct _serviceProduct;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ServiceProduct serviceProduct, ILogger<ProductController> logger)
        {
            _serviceProduct = serviceProduct ?? throw new ArgumentNullException(nameof(serviceProduct));
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            try
            {
                var createdProduct = await _serviceProduct.AddProduct(product);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product.");
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                var products = _serviceProduct.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products.");
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            try
            {
                var product = _serviceProduct.GetProductById(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found.", id);
                    return NotFound(new Response { Status = "Error", Message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                _serviceProduct.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct(int id, Product product)
        {
            try
            {
                _serviceProduct.UpdateProduct(id, product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }
    }
}
