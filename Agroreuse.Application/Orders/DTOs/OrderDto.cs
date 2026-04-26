using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.Orders.DTOs
{
    /// <summary>
    /// Data transfer object for Order.
    /// </summary>
    public record OrderDto(
        Guid Id,
        string SellerId,
        string SellerName,
        Guid AddressId,
        string AddressDetails,
        Guid CategoryId,
        string CategoryName,
        string? Description,
        int Quantity,
        string NumberOfDays,
        OrderStatus Status,
        DateTime CreatedAt,
        List<string> ImagePaths);
}
