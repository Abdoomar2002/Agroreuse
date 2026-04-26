using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.Orders.Queries.GetOrdersBySeller
{
    /// <summary>
    /// Handler for GetOrdersBySellerQuery.
    /// </summary>
    public class GetOrdersBySellerQueryHandler : IQueryHandler<GetOrdersBySellerQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IOrderRepository _repository;

        public GetOrdersBySellerQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(
            GetOrdersBySellerQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _repository.GetBySellerIdAsync(request.SellerId, cancellationToken);

            return orders.Select(order => new OrderDto(
                order.Id,
                order.SellerId,
                order.Seller?.FullName ?? string.Empty,
                order.AddressId,
                order.Address?.Details ?? string.Empty,
                order.CategoryId,
                order.Category?.Name ?? string.Empty,
                order.Description,
                order.Quantity,
                order.NumberOfDays,
                order.Status,
                order.CreatedAt,
                order.Images.Select(i => i.ImagePath).ToList())).ToList();
        }
    }
}
