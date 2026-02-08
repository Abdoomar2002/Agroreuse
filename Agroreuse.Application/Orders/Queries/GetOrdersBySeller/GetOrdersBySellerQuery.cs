using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;

namespace Agroreuse.Application.Orders.Queries.GetOrdersBySeller
{
    /// <summary>
    /// Query to get all Orders by seller ID.
    /// </summary>
    public record GetOrdersBySellerQuery(string SellerId) : IQuery<IReadOnlyList<OrderDto>>;
}
