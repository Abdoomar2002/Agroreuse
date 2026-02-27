using Agroreuse.Application.Orders.Commands.CreateOrder;
using Agroreuse.Application.Orders.Commands.DeleteOrder;
using Agroreuse.Application.Orders.Commands.UpdateOrder;
using Agroreuse.Application.Orders.DTOs;
using Agroreuse.Application.Orders.Queries.GetAllOrders;
using Agroreuse.Application.Orders.Queries.GetOrderById;
using Agroreuse.Application.Orders.Queries.GetOrdersBySeller;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
using Agroreuse.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ArgoreuseContext _context;
        private readonly IFileUploadService _fileUploadService;

        public OrdersController(
            IMediator mediator,
            ArgoreuseContext context,
            IFileUploadService fileUploadService)
        {
            _mediator = mediator;
            _context = context;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Get aggregate statistics for orders (Admin only)
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrdersStatistics()
        {
            try
            {
                // total orders
                var total = await _context.Orders.CountAsync();

                // counts per status (materialize to memory to translate enum names)
                var countsByStatus = _context.Orders
                    .AsEnumerable()
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToDictionary(k => k.Status, v => v.Count);

                // total and average quantity
                var totalQuantity = await _context.Orders.SumAsync(o => (int?)o.Quantity) ?? 0;
                var avgQuantity = total > 0 ? await _context.Orders.AverageAsync(o => o.Quantity) : 0;

                // orders per month for last 6 months
                var now = DateTime.UtcNow;
                var months = Enumerable.Range(0, 6).Select(i => now.AddMonths(-i)).Reverse();
                var ordersByMonth = months.Select(m => new
                {
                    Month = m.ToString("yyyy-MM"),
                    Count = _context.Orders.Count(o => o.CreatedAt.Year == m.Year && o.CreatedAt.Month == m.Month)
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        TotalOrders = total,
                        CountsByStatus = countsByStatus,
                        TotalQuantity = totalQuantity,
                        AverageQuantity = avgQuantity,
                        OrdersByMonth = ordersByMonth
                    },
                    Message = "Order statistics retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get aggregate statistics for the current seller (authenticated user)
        /// </summary>
        [HttpGet("my-stats")]
        public async Task<IActionResult> GetMyOrdersStatistics()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "User not authenticated."
                    });

                var total = await _context.Orders.CountAsync(o => o.SellerId == userId);

                var countsByStatus = _context.Orders
                    .Where(o => o.SellerId == userId)
                    .AsEnumerable()
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToDictionary(k => k.Status, v => v.Count);

                var totalQuantity = await _context.Orders.Where(o => o.SellerId == userId).SumAsync(o => (int?)o.Quantity) ?? 0;
                var avgQuantity = total > 0 ? await _context.Orders.Where(o => o.SellerId == userId).AverageAsync(o => o.Quantity) : 0;

                var now = DateTime.UtcNow;
                var months = Enumerable.Range(0, 6).Select(i => now.AddMonths(-i)).Reverse();
                var ordersByMonth = months.Select(m => new
                {
                    Month = m.ToString("yyyy-MM"),
                    Count = _context.Orders.Count(o => o.SellerId == userId && o.CreatedAt.Year == m.Year && o.CreatedAt.Month == m.Month)
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        TotalOrders = total,
                        CountsByStatus = countsByStatus,
                        TotalQuantity = totalQuantity,
                        AverageQuantity = avgQuantity,
                        OrdersByMonth = ordersByMonth
                    },
                    Message = "Your order statistics retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var query = new GetAllOrdersQuery();
                var orders = await _mediator.Send(query);

                return Ok(new
                {
                    Success = true,
                    Data = orders,
                    Message = "Orders retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
                var order = await _mediator.Send(query);

                if (order == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Order not found."
                    });

                return Ok(new
                {
                    Success = true,
                    Data = order,
                    Message = "Order retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get orders by current user (seller)
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "User not authenticated."
                    });

                var query = new GetOrdersBySellerQuery(userId);
                var orders = await _mediator.Send(query);

                return Ok(new
                {
                    Success = true,
                    Data = orders,
                    Message = "Your orders retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get orders by specific seller ID (Admin only)
        /// </summary>
        [HttpGet("seller/{sellerId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrdersBySeller(string sellerId)
        {
            try
            {
                var query = new GetOrdersBySellerQuery(sellerId);
                var orders = await _mediator.Send(query);

                return Ok(new
                {
                    Success = true,
                    Data = orders,
                    Message = "Seller orders retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request data.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });

                if (request.Address == null)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Address is required."
                    });

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "User not authenticated."
                    });

                var addressDto = new CreateOrderAddressDto(
                    request.Address.GovernmentId,
                    request.Address.CityId,
                    request.Address.Details);

                var command = new CreateOrderCommand(
                    userId,
                    addressDto,
                    request.CategoryId,
                    request.Quantity,
                    request.NumberOfDays,
                    request.ImagePaths);

                var orderId = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new
                {
                    Success = true,
                    Data = new { Id = orderId },
                    Message = "Order created successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request data.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors)
                    });

                var command = new UpdateOrderCommand(
                    id,
                    request.AddressId,
                    request.CategoryId,
                    request.Quantity,
                    request.NumberOfDays,
                    request.Status);

                await _mediator.Send(command);

                return Ok(new
                {
                    Success = true,
                    Message = "Order updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                var command = new DeleteOrderCommand(id);
                await _mediator.Send(command);

                return Ok(new
                {
                    Success = true,
                    Message = "Order deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #region Image Management

        /// <summary>
        /// Get all images for an order
        /// </summary>
        [HttpGet("{orderId}/images")]
        public async Task<IActionResult> GetOrderImages(Guid orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Images)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Order not found."
                    });

                var images = order.Images.Select(i => new
                {
                    i.Id,
                    i.ImagePath
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Data = images,
                    Message = "Order images retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Upload images to an order (max 4 images)
        /// </summary>
        [HttpPost("{orderId}/images")]
        public async Task<IActionResult> UploadOrderImages([FromQuery]Guid orderId, List<IFormFile>? images)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Images)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Order not found."
                    });

                if (images == null || images.Count == 0)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "No images provided."
                    });

                // Check if adding new images would exceed the limit
                int availableSlots = 4 - order.Images.Count;
                if (availableSlots <= 0)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Order already has the maximum number of images (4)."
                    });

                if (images.Count > availableSlots)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Can only add {availableSlots} more images. Order already has {order.Images.Count} images."
                    });

                var uploadedImages = new List<object>();

                foreach (var image in images)
                {
                    if (image.Length == 0)
                        continue;

                    if (!_fileUploadService.IsValidImage(image))
                        return BadRequest(new
                        {
                            Success = false,
                            Message = $"Invalid image format or size for file '{image.FileName}'."
                        });

                    string imagePath = await _fileUploadService.UploadImageAsync(image, "orders");
                    var orderImage = new OrderImage { ImagePath = imagePath, OrderId = order.Id };

                    // Explicitly add to DbContext to ensure proper tracking
                    await _context.OrderImages.AddAsync(orderImage);

                    uploadedImages.Add(new
                    {
                        Id = orderImage.Id,
                        ImagePath = imagePath
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Data = uploadedImages,
                    Message = $"{uploadedImages.Count} image(s) uploaded successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete a specific image from an order
        /// </summary>
        [HttpDelete("{orderId}/images/{imageId}")]
        public async Task<IActionResult> DeleteOrderImage(Guid orderId, Guid imageId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Images)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Order not found."
                    });

                var image = order.Images.FirstOrDefault(i => i.Id == imageId);
                if (image == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Image not found."
                    });

                // Delete the file from storage
                _fileUploadService.DeleteImage(image.ImagePath);

                // Remove from database using DbContext
                _context.OrderImages.Remove(image);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Image deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Replace all images for an order
        /// </summary>
        [HttpPut("{orderId}/images")]
        public async Task<IActionResult> ReplaceOrderImages(Guid orderId,  List<IFormFile>? images)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Images)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Order not found."
                    });

                if (images == null || images.Count == 0)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "No images provided."
                    });

                if (images.Count > 4)
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Maximum 4 images allowed per order."
                    });

                // Delete old images from storage and database
                foreach (var oldImage in order.Images.ToList())
                {
                    _fileUploadService.DeleteImage(oldImage.ImagePath);
                    _context.OrderImages.Remove(oldImage);
                }

                var uploadedImages = new List<object>();

                // Upload new images
                foreach (var image in images)
                {
                    if (image.Length == 0)
                        continue;

                    if (!_fileUploadService.IsValidImage(image))
                        return BadRequest(new
                        {
                            Success = false,
                            Message = $"Invalid image format or size for file '{image.FileName}'."
                        });

                    string imagePath = await _fileUploadService.UploadImageAsync(image, "orders");
                    var orderImage = new OrderImage { ImagePath = imagePath, OrderId = order.Id };

                    // Explicitly add to DbContext to ensure proper tracking
                    await _context.OrderImages.AddAsync(orderImage);

                    uploadedImages.Add(new
                    {
                        Id = orderImage.Id,
                        ImagePath = imagePath
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Data = uploadedImages,
                    Message = "Order images replaced successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        #endregion
    }

    /// <summary>
    /// Request model for creating an order
    /// </summary>
    public class CreateOrderRequest
    {
        public AddressRequest Address { get; set; }
        public Guid CategoryId { get; set; }
        public int Quantity { get; set; }
        public string NumberOfDays { get; set; }
        public List<string>? ImagePaths { get; set; }
    }

    /// <summary>
    /// Address request model
    /// </summary>
    public class AddressRequest
    {
        public Guid GovernmentId { get; set; }
        public Guid CityId { get; set; }
        public string Details { get; set; }
    }

    /// <summary>
    /// Request model for updating an order
    /// </summary>
    public class UpdateOrderRequest
    {
        public Guid AddressId { get; set; }
        public Guid CategoryId { get; set; }
        public int Quantity { get; set; }
        public string NumberOfDays { get; set; }
        public OrderStatus Status { get; set; }
    }
}
