using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Vacancies.Data;
using Vacancies.DTOs;
using Vacancies.Models;

namespace Vacancies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GrantsController> _logger;

        public GrantsController(ApplicationDbContext context, ILogger<GrantsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/grants
        [HttpGet]
        public async Task<ActionResult<PagedResult<GrantDTO>>> GetGrants(
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string? country = null,
            [FromQuery] bool activeOnly = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                _logger.LogInformation("Fetching grants with filters - CategoryId: {CategoryId}, Country: {Country}, ActiveOnly: {ActiveOnly}, Page: {Page}, PageSize: {PageSize}", 
                    categoryId, country, activeOnly, page, pageSize);

                var query = _context.Grants
                    .Include(g => g.Categories)
                    .AsQueryable();

                if (categoryId.HasValue)
                    query = query.Where(g => g.Categories.Any(c => c.Id == categoryId));

                if (!string.IsNullOrEmpty(country))
                    query = query.Where(g => g.Country.ToLower().Contains(country.ToLower()));

                if (activeOnly)
                    query = query.Where(g => g.IsActive && g.Deadline > DateTime.UtcNow);

                var totalCount = await query.CountAsync();

                var grants = await query
                    .OrderByDescending(g => g.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => MapToGrantDTO(g))
                    .ToListAsync();

                var result = new PagedResult<GrantDTO>
                {
                    Items = grants,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                _logger.LogInformation("Successfully fetched {Count} grants out of {TotalCount} total grants", grants.Count, totalCount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching grants");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // GET: api/grants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GrantDTO>> GetGrant(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid grant ID provided: {Id}", id);
                    return BadRequest("Invalid grant ID");
                }

                _logger.LogInformation("Fetching grant with ID: {Id}", id);

                var grant = await _context.Grants
                    .Include(g => g.Categories)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (grant == null)
                {
                    _logger.LogWarning("Grant with ID {Id} not found", id);
                    return NotFound($"Grant with ID {id} not found");
                }

                _logger.LogInformation("Successfully fetched grant with ID: {Id}", id);
                return Ok(MapToGrantDTO(grant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching grant with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // POST: api/grants
        [HttpPost]
        public async Task<ActionResult<GrantDTO>> CreateGrant([FromBody] CreateGrantDTO createGrantDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for create grant request");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new grant with title: {Title}", createGrantDto.Title);

                var categories = createGrantDto.CategoryIds != null && createGrantDto.CategoryIds.Any()
                    ? await _context.Categories
                        .Where(c => createGrantDto.CategoryIds.Contains(c.Id))
                        .ToListAsync()
                    : new List<Category>();

                // Validate that all provided category IDs exist
                if (createGrantDto.CategoryIds != null && createGrantDto.CategoryIds.Count != categories.Count)
                {
                    _logger.LogWarning("Some category IDs provided do not exist");
                    return BadRequest("One or more category IDs do not exist");
                }

                var grant = new Grant
                {
                    Title = createGrantDto.Title,
                    Description = createGrantDto.Description,
                    Country = createGrantDto.Country,
                    Deadline = createGrantDto.Deadline,
                    Requirements = createGrantDto.Requirements,
                    FundingAmount = createGrantDto.FundingAmount,
                    Categories = categories
                };

                _context.Grants.Add(grant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created grant with ID: {Id}", grant.Id);

                return CreatedAtAction(
                    nameof(GetGrant),
                    new { id = grant.Id },
                    MapToGrantDTO(grant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating grant");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // PUT: api/grants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrant(int id, [FromBody] CreateGrantDTO updateGrantDto)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid grant ID provided for update: {Id}", id);
                    return BadRequest("Invalid grant ID");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for update grant request");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating grant with ID: {Id}", id);

                var grant = await _context.Grants
                    .Include(g => g.Categories)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (grant == null)
                {
                    _logger.LogWarning("Grant with ID {Id} not found for update", id);
                    return NotFound($"Grant with ID {id} not found");
                }

                // Validate categories
                var categories = updateGrantDto.CategoryIds != null && updateGrantDto.CategoryIds.Any()
                    ? await _context.Categories
                        .Where(c => updateGrantDto.CategoryIds.Contains(c.Id))
                        .ToListAsync()
                    : new List<Category>();

                if (updateGrantDto.CategoryIds != null && updateGrantDto.CategoryIds.Count != categories.Count)
                {
                    _logger.LogWarning("Some category IDs provided do not exist during update");
                    return BadRequest("One or more category IDs do not exist");
                }

                // Update grant properties
                grant.Title = updateGrantDto.Title;
                grant.Description = updateGrantDto.Description;
                grant.Country = updateGrantDto.Country;
                grant.Deadline = updateGrantDto.Deadline;
                grant.Requirements = updateGrantDto.Requirements;
                grant.FundingAmount = updateGrantDto.FundingAmount;
                grant.UpdatedAt = DateTime.UtcNow;

                // Update categories
                grant.Categories.Clear();
                foreach (var category in categories)
                {
                    grant.Categories.Add(category);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated grant with ID: {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict occurred while updating grant with ID: {Id}", id);
                if (!await GrantExistsAsync(id))
                    return NotFound($"Grant with ID {id} not found");
                return Conflict("The grant was modified by another user. Please reload and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating grant with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // DELETE: api/grants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrant(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid grant ID provided for deletion: {Id}", id);
                    return BadRequest("Invalid grant ID");
                }

                _logger.LogInformation("Deleting grant with ID: {Id}", id);

                var grant = await _context.Grants.FindAsync(id);
                if (grant == null)
                {
                    _logger.LogWarning("Grant with ID {Id} not found for deletion", id);
                    return NotFound($"Grant with ID {id} not found");
                }

                _context.Grants.Remove(grant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted grant with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting grant with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private async Task<bool> GrantExistsAsync(int id)
        {
            return await _context.Grants.AnyAsync(e => e.Id == id);
        }

        private static GrantDTO MapToGrantDTO(Grant grant)
        {
            return new GrantDTO
            {
                Id = grant.Id,
                Title = grant.Title,
                Description = grant.Description,
                Country = grant.Country,
                Deadline = grant.Deadline,
                Requirements = grant.Requirements,
                FundingAmount = grant.FundingAmount,
                Categories = grant.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };
        }
    }
}
