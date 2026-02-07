using Agroreuse.Application.DTOs;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ArgoreuseContext _context;

        public CitiesController(ArgoreuseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCities([FromQuery] Guid? governmentId = null)
        {
            var query = _context.Cities.Include(c => c.Government).AsQueryable();

            if (governmentId.HasValue)
            {
                query = query.Where(c => c.GovernmentId == governmentId.Value);
            }

            var cities = await query
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    GovernmentId = c.GovernmentId,
                    GovernmentName = c.Government.Name
                })
                .ToListAsync();

            return Ok(cities);
        }

        /// <summary>
        /// Get city by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCity(Guid id)
        {
            var city = await _context.Cities
                .Include(c => c.Government)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return NotFound(new { Success = false, Message = "City not found." });

            return Ok(new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                GovernmentId = city.GovernmentId,
                GovernmentName = city.Government.Name
            });
        }

        /// <summary>
        /// Create new city (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCity([FromBody] CityDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest(new { Success = false, Message = "City name is required." });

            // Validate government exists
            var government = await _context.Governments.FindAsync(dto.GovernmentId);
            if (government == null)
                return BadRequest(new { Success = false, Message = "Government not found." });

            var city = new City(dto.Name, dto.GovernmentId);
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                GovernmentId = city.GovernmentId,
                GovernmentName = government.Name
            });
        }

        /// <summary>
        /// Update city (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCity(Guid id, [FromBody] CityDto dto)
        {
            var city = await _context.Cities.Include(c => c.Government).FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return NotFound(new { Success = false, Message = "City not found." });

            if (!string.IsNullOrEmpty(dto.Name))
                city.Name = dto.Name;

            if (dto.GovernmentId != Guid.Empty && dto.GovernmentId != city.GovernmentId)
            {
                var government = await _context.Governments.FindAsync(dto.GovernmentId);
                if (government == null)
                    return BadRequest(new { Success = false, Message = "Government not found." });

                city.GovernmentId = dto.GovernmentId;
            }

            await _context.SaveChangesAsync();

            // Reload to get updated government name
            await _context.Entry(city).Reference(c => c.Government).LoadAsync();

            return Ok(new
            {
                Success = true,
                Message = "City updated successfully.",
                Data = new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    GovernmentId = city.GovernmentId,
                    GovernmentName = city.Government.Name
                }
            });
        }

        /// <summary>
        /// Delete city (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
                return NotFound(new { Success = false, Message = "City not found." });

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "City deleted successfully." });
        }
    }
}
