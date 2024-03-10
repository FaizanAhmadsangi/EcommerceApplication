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
    public class CategoryController : ControllerBase
    {
        private readonly ServiceCategory _serviceCategory;
        private readonly ILogger<CategoryController> _logger; // Add ILogger

        public CategoryController(ServiceCategory serviceCategory, ILogger<CategoryController> logger)
        {
            _serviceCategory = serviceCategory ?? throw new ArgumentNullException(nameof(serviceCategory));
            _logger = logger; // Injected ILogger
        }

        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            try
            {
                var createdCategory = await _serviceCategory.AddCategory(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding category.");
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                var categories = _serviceCategory.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories.");
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Category> GetCategoryById(int id)
        {
            try
            {
                var category = _serviceCategory.GetCategoryById(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found.", id);
                    return NotFound(new Response { Status = "Error", Message = "Category not found" });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                _serviceCategory.DeleteCategory(id);
                _logger.LogInformation("Category with ID {Id} deleted successfully.", id);
                return Ok(new Response { Status = "Success", Message = "Category deleted successfully and all linked products also" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCategory(int id, Category category)
        {
            try
            {
                _serviceCategory.UpdateCategory(id, category);
                _logger.LogInformation("Category with ID {Id} updated successfully.", id);
                return Ok(new Response { Status = "Success", Message = "Category updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID {Id}.", id);
                return StatusCode(500, new Response { Status = "Error", Message = ex.Message });
            }
        }

    }
}
