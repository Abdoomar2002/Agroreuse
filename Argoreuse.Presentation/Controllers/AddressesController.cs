using Agroreuse.Application.DTOs;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly ArgoreuseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressesController(ArgoreuseContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all addresses (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await _context.Addresses
                .Include(a => a.Government)
                .Include(a => a.City)
                .Include(a => a.ApplicationUser)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    GovernmentId = a.GovernmentId,
                    GovernmentName = a.Government.Name,
                    CityId = a.CityId,
                    CityName = a.City.Name,
                    Details = a.Details
                })
                .ToListAsync();

            return Ok(addresses);
        }

        /// <summary>
        /// Get current user's address
        /// </summary>
        [HttpGet("my-address")]
        public async Task<IActionResult> GetMyAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var address = await _context.Addresses
                .Include(a => a.Government)
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (address == null)
                return NotFound(new { Success = false, Message = "No address found." });

            return Ok(new AddressDto
            {
                Id = address.Id,
                GovernmentId = address.GovernmentId,
                GovernmentName = address.Government.Name,
                CityId = address.CityId,
                CityName = address.City.Name,
                Details = address.Details
            });
        }

        /// <summary>
        /// Get address by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAddress(Guid id)
        {
            var address = await _context.Addresses
                .Include(a => a.Government)
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (address == null)
                return NotFound(new { Success = false, Message = "Address not found." });

            return Ok(new AddressDto
            {
                Id = address.Id,
                GovernmentId = address.GovernmentId,
                GovernmentName = address.Government.Name,
                CityId = address.CityId,
                CityName = address.City.Name,
                Details = address.Details
            });
        }

        /// <summary>
        /// Create or update current user's address
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateMyAddress([FromBody] AddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            // Validate government and city
            var government = await _context.Governments.FindAsync(dto.GovernmentId);
            if (government == null)
                return BadRequest(new { Success = false, Message = "Government not found." });

            var city = await _context.Cities.FindAsync(dto.CityId);
            if (city == null)
                return BadRequest(new { Success = false, Message = "City not found." });

            if (city.GovernmentId != dto.GovernmentId)
                return BadRequest(new { Success = false, Message = "City does not belong to the specified government." });

            // Check if user already has an address
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (existingAddress != null)
            {
                // Update existing address
                existingAddress.GovernmentId = dto.GovernmentId;
                existingAddress.CityId = dto.CityId;
                existingAddress.Details = dto.Details;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Address updated successfully.",
                    Data = new AddressDto
                    {
                        Id = existingAddress.Id,
                        GovernmentId = existingAddress.GovernmentId,
                        GovernmentName = government.Name,
                        CityId = existingAddress.CityId,
                        CityName = city.Name,
                        Details = existingAddress.Details
                    }
                });
            }
            else
            {
                // Create new address
                var address = new Address(dto.GovernmentId, dto.CityId, dto.Details)
                {
                    ApplicationUserId = userId
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMyAddress), null, new
                {
                    Success = true,
                    Message = "Address created successfully.",
                    Data = new AddressDto
                    {
                        Id = address.Id,
                        GovernmentId = address.GovernmentId,
                        GovernmentName = government.Name,
                        CityId = address.CityId,
                        CityName = city.Name,
                        Details = address.Details
                    }
                });
            }
        }

        /// <summary>
        /// Delete current user's address
        /// </summary>
        [HttpDelete("my-address")]
        public async Task<IActionResult> DeleteMyAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (address == null)
                return NotFound(new { Success = false, Message = "No address found." });

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Address deleted successfully." });
        }

        /// <summary>
        /// Delete address by ID (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
                return NotFound(new { Success = false, Message = "Address not found." });

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Address deleted successfully." });
        }
    }
}
