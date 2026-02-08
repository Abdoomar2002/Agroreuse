using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;

namespace Agroreuse.Application.Orders.Queries.GetAllOrders
{
    /// <summary>
    /// Query to get all Orders.
    /// </summary>
    public record GetAllOrdersQuery : IQuery<IReadOnlyList<OrderDto>>;
}
