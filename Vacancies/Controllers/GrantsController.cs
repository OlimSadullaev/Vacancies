using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public GrantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/grants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GrantDTO>>> GetGrants(
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string country = null,
            [FromQuery] bool activeOnly = true)
        {
            var query = _context.Grants
                .Include(g => g.Categories)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(g => g.Categories.Any(c => c.Id == categoryId));

            if (!string.IsNullOrEmpty(country))
                query = query.Where(g => g.Country == country);

            if (activeOnly)
                query = query.Where(g => g.IsActive && g.Deadline > DateTime.UtcNow);

            var grants = await query
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => new GrantDTO
                {
                    Id = g.Id,
                    Title = g.Title,
                    Description = g.Description,
                    Country = g.Country,
                    Deadline = g.Deadline,
                    Requirements = g.Requirements,
                    FundingAmount = g.FundingAmount,
                    Categories = g.Categories.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList()
                })
                .ToListAsync();

            return Ok(grants);
        }

        // GET: api/grants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GrantDTO>> GetGrant(int id)
        {
            var grant = await _context.Grants
                .Include(g => g.Categories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grant == null)
                return NotFound();

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

        // POST: api/grants
        [HttpPost]
        public async Task<ActionResult<GrantDTO>> CreateGrant(CreateGrantDTO createGrantDto)
        {
            var categories = createGrantDto.CategoryIds != null && createGrantDto.CategoryIds.Any()
                ? await _context.Categories
                    .Where(c => createGrantDto.CategoryIds.Contains(c.Id))
                    .ToListAsync()
                : new List<Category>();

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

            return CreatedAtAction(
                nameof(GetGrant),
                new { id = grant.Id },
                new GrantDTO
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
                });
        }

        // PUT: api/grants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrant(int id, CreateGrantDTO updateGrantDto)
        {
            var grant = await _context.Grants
                .Include(g => g.Categories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grant == null)
                return NotFound();

            grant.Title = updateGrantDto.Title;
            grant.Description = updateGrantDto.Description;
            grant.Country = updateGrantDto.Country;
            grant.Deadline = updateGrantDto.Deadline;
            grant.Requirements = updateGrantDto.Requirements;
            grant.FundingAmount = updateGrantDto.FundingAmount;
            grant.UpdatedAt = DateTime.UtcNow;

            var categories = await _context.Categories
                .Where(c => updateGrantDto.CategoryIds.Contains(c.Id))
                .ToListAsync();

            grant.Categories.Clear();
            foreach (var category in categories)a
            {
                grant.Categories.Add(category);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrantExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/grants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrant(int id)
        {
            var grant = await _context.Grants.FindAsync(id);
            if (grant == null)
                return NotFound();

            _context.Grants.Remove(grant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrantExists(int id)
        {
            return _context.Grants.Any(e => e.Id == id);
        }
    }
}
