using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;

namespace Agroreuse.Application.Orders.Queries.GetOrderById
{
    /// <summary>
    /// Query to get an Order by its ID.
    /// </summary>
    public record GetOrderByIdQuery(Guid Id) : IQuery<OrderDto?>;
}
