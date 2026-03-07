using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.Orders.Queries.GetOrderById
{
    /// <summary>
    /// Handler for GetOrderByIdQuery.
    /// </summary>
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _repository;

        public GetOrderByIdQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (order is null)
                return null;

            return new OrderDto(
                order.Id,
                order.SellerId,
                order.Seller?.FullName ?? string.Empty,
                order.AddressId,
                order.Address?.Details ?? string.Empty,
                order.CategoryId,
                order.Category?.Name ?? string.Empty,
                order.Quantity,
                order.NumberOfDays,
                order.Status,
                order.CreatedAt,
                order.Images.Select(i => i.ImagePath).ToList());
        }
    }
}
