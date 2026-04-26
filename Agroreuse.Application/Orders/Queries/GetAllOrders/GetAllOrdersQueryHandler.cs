using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.Orders.DTOs;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.Orders.Queries.GetAllOrders
{
    /// <summary>
    /// Handler for GetAllOrdersQuery.
    /// </summary>
    public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IOrderRepository _repository;

        public GetAllOrdersQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(
            GetAllOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _repository.GetAllWithDetailsAsync(cancellationToken);

            return orders.Select(order => new OrderDto(
                order.Id,
                order.SellerId,
                order.Seller?.FullName ?? string.Empty,
                order.AddressId,
               AddressDetails:order.Address.Government.Name+" , "+order.Address.City.Name+" , "+ order.Address?.Details ?? string.Empty,
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
