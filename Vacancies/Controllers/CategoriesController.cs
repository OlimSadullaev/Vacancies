using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Vacancies.Data;
using Vacancies.DTOs;
using Vacancies.Models;

namespace Vacancies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ApplicationDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryDTO>>> GetCategories(
            [FromQuery] string search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                _logger.LogInformation("Fetching categories with search: {Search}, Page: {Page}, PageSize: {PageSize}", 
                    search, page, pageSize);

                var query = _context.Categories.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()) ||
                                           c.Description.ToLower().Contains(search.ToLower()));
                }

                var totalCount = await query.CountAsync();

                var categories = await query
                    .OrderBy(c => c.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => MapToCategoryDTO(c))
                    .ToListAsync();

                var result = new PagedResult<CategoryDTO>
                {
                    Items = categories,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                _logger.LogInformation("Successfully fetched {Count} categories out of {TotalCount} total categories", 
                    categories.Count, totalCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid category ID provided: {Id}", id);
                    return BadRequest("Invalid category ID");
                }

                _logger.LogInformation("Fetching category with ID: {Id}", id);

                var category = await _context.Categories
                    .Include(c => c.Grants)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found", id);
                    return NotFound($"Category with ID {id} not found");
                }

                _logger.LogInformation("Successfully fetched category with ID: {Id}", id);
                return Ok(MapToCategoryDTO(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")] // Enable proper authorization
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO createCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for create category request");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new category with name: {Name}", createCategoryDto.Name);

                // Check if category with same name already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == createCategoryDto.Name.ToLower());

                if (existingCategory != null)
                {
                    _logger.LogWarning("Category with name '{Name}' already exists", createCategoryDto.Name);
                    return BadRequest("A category with this name already exists");
                }

                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created category with ID: {Id}", category.Id);

                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = category.Id },
                    MapToCategoryDTO(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Enable proper authorization
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryDTO updateCategoryDto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid category ID provided for update: {Id}", id);
                    return BadRequest("Invalid category ID");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for update category request");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating category with ID: {Id}", id);

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found for update", id);
                    return NotFound($"Category with ID {id} not found");
                }

                // Check if another category with same name already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == updateCategoryDto.Name.ToLower() && c.Id != id);

                if (existingCategory != null)
                {
                    _logger.LogWarning("Another category with name '{Name}' already exists", updateCategoryDto.Name);
                    return BadRequest("A category with this name already exists");
                }

                category.Name = updateCategoryDto.Name;
                category.Description = updateCategoryDto.Description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated category with ID: {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating category with ID: {Id}", id);
                if (!await CategoryExistsAsync(id))
                    return NotFound($"Category with ID {id} not found");
                return Conflict("The category was modified by another user. Please reload and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Enable proper authorization
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid category ID provided for deletion: {Id}", id);
                    return BadRequest("Invalid category ID");
                }

                _logger.LogInformation("Deleting category with ID: {Id}", id);

                var category = await _context.Categories
                    .Include(c => c.Grants)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found for deletion", id);
                    return NotFound($"Category with ID {id} not found");
                }

                // Check if category has associated grants
                if (category.Grants.Any())
                {
                    _logger.LogWarning("Cannot delete category with ID {Id} as it has associated grants", id);
                    return BadRequest("Cannot delete category that has associated grants");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted category with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await _context.Categories.AnyAsync(e => e.Id == id);
        }

        private static CategoryDTO MapToCategoryDTO(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
