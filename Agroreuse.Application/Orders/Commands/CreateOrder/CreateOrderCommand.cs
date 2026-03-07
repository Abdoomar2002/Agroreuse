using Agroreuse.Application.Common.Commands;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Command to create a new Order.
    /// </summary>
    public record CreateOrderCommand(
        string SellerId,
        CreateOrderAddressDto Address,
        Guid CategoryId,
        int Quantity,
        string NumberOfDays,
        List<string>? ImagePaths) : ICommand<Guid>;

    /// <summary>
    /// Address data for creating an order
    /// </summary>
    public record CreateOrderAddressDto(
        Guid GovernmentId,
        Guid CityId,
        string Details);
}
