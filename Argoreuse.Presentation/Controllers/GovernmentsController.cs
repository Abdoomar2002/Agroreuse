using Agroreuse.Application.DTOs;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agroreuse.Domain.Entities;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentsController : ControllerBase
    {
        private readonly ArgoreuseContext _context;

        public GovernmentsController(ArgoreuseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all governments with their cities
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetGovernments([FromQuery] bool includeCities = false)
        {
            var query = _context.Governments.AsQueryable();

            if (includeCities)
            {
                query = query.Include(g => g.Cities);
            }

            var governments = await query
                .Select(g => new GovernmentDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Cities = includeCities ? g.Cities.Select(c => new CityDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        GovernmentId = c.GovernmentId
                    }).ToList() : null
                })
                .ToListAsync();

            return Ok(governments);
        }

        /// <summary>
        /// Get government by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGovernment(Guid id, [FromQuery] bool includeCities = true)
        {
            var query = _context.Governments.AsQueryable();

            if (includeCities)
            {
                query = query.Include(g => g.Cities);
            }

            var government = await query.FirstOrDefaultAsync(g => g.Id == id);

            if (government == null)
                return NotFound(new { Success = false, Message = "Government not found." });

            return Ok(new GovernmentDto
            {
                Id = government.Id,
                Name = government.Name,
                Cities = includeCities ? government.Cities.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    GovernmentId = c.GovernmentId
                }).ToList() : null
            });
        }

        /// <summary>
        /// Create new government (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateGovernment([FromBody] GovernmentDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest(new { Success = false, Message = "Government name is required." });

            var government = new Government(dto.Name);
            _context.Governments.Add(government);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGovernment), new { id = government.Id }, new GovernmentDto
            {
                Id = government.Id,
                Name = government.Name
            });
        }

        /// <summary>
        /// Update government (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateGovernment(Guid id, [FromBody] GovernmentDto dto)
        {
            var government = await _context.Governments.FindAsync(id);

            if (government == null)
                return NotFound(new { Success = false, Message = "Government not found." });

            if (!string.IsNullOrEmpty(dto.Name))
                government.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Government updated successfully.",
                Data = new GovernmentDto
                {
                    Id = government.Id,
                    Name = government.Name
                }
            });
        }

        /// <summary>
        /// Delete government (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteGovernment(Guid id)
        {
            var government = await _context.Governments
                .Include(g => g.Cities)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (government == null)
                return NotFound(new { Success = false, Message = "Government not found." });

            if (government.Cities.Any())
                return BadRequest(new { Success = false, Message = "Cannot delete government with existing cities." });

            _context.Governments.Remove(government);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Government deleted successfully." });
        }
    }
}
