using Agroreuse.Application.Common.Commands;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.Orders.Commands.UpdateOrder
{
    /// <summary>
    /// Command to update an existing Order.
    /// </summary>
    public record UpdateOrderCommand(
        Guid Id,
        Guid AddressId,
        Guid CategoryId,
        string? Description,
        int Quantity,
        string NumberOfDays,
        OrderStatus Status) : ICommand;
}
