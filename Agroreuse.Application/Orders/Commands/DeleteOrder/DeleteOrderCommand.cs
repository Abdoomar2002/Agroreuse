using Agroreuse.Application.Common.Commands;

namespace Agroreuse.Application.Orders.Commands.DeleteOrder
{
    /// <summary>
    /// Command to delete an Order.
    /// </summary>
    public record DeleteOrderCommand(Guid Id) : ICommand;
}
