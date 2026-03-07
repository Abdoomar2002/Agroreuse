using Agroreuse.Application.DTOs;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ArgoreuseContext _context;
        private readonly IFileUploadService _fileUploadService;

        public CategoriesController(ArgoreuseContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImagePath = c.ImagePath
                })
                .ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound(new { Success = false, Message = "Category not found." });

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath
            });
        }

        /// <summary>
        /// Create new category (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCategory([FromQuery] string name,  IFormFile? image)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest(new { Success = false, Message = "Category name is required." });

            string? imagePath = null;

            // Upload image if provided
            if (image != null && image.Length > 0)
            {
                if (!_fileUploadService.IsValidImage(image))
                    return BadRequest(new { Success = false, Message = "Invalid image format or size." });

                imagePath = await _fileUploadService.UploadImageAsync(image, "categories");
            }

            var category = new Category(name, imagePath);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath
            });
        }

        /// <summary>
        /// Update category (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] string? name, [FromForm] IFormFile? image)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound(new { Success = false, Message = "Category not found." });

            // Update name if provided
            if (!string.IsNullOrEmpty(name))
                category.Name = name;

            // Update image if provided
            if (image != null && image.Length > 0)
            {
                if (!_fileUploadService.IsValidImage(image))
                    return BadRequest(new { Success = false, Message = "Invalid image format or size." });

                // Delete old image
                if (!string.IsNullOrEmpty(category.ImagePath))
                    _fileUploadService.DeleteImage(category.ImagePath);

                category.ImagePath = await _fileUploadService.UploadImageAsync(image, "categories");
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Category updated successfully.",
                Data = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImagePath = category.ImagePath
                }
            });
        }

        /// <summary>
        /// Delete category (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound(new { Success = false, Message = "Category not found." });

            // Delete image if exists
            if (!string.IsNullOrEmpty(category.ImagePath))
                _fileUploadService.DeleteImage(category.ImagePath);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Category deleted successfully." });
        }
    }
}
