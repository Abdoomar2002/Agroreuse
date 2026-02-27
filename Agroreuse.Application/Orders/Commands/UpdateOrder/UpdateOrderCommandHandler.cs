using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Exceptions;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.Orders.Commands.UpdateOrder
{
    /// <summary>
    /// Handler for UpdateOrderCommand.
    /// </summary>
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Agroreuse.Application.Services.INotificationService _notificationService;

        public UpdateOrderCommandHandler(
            IOrderRepository repository,
            IUnitOfWork unitOfWork,
            Agroreuse.Application.Services.INotificationService notificationService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.Id);

            var previousStatus = order.Status;

            order.Update(
                request.AddressId,
                request.CategoryId,
                request.Quantity,
                request.NumberOfDays,
                request.Status);

            await _repository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // If status changed, create notification and send via SignalR
            if (previousStatus != order.Status)
            {
                var notification = new Agroreuse.Domain.Entities.Notification
                {
                    RecipientId = order.SellerId,
                    Title = "Order Status Updated",
                    Message = $"Your order {order.Id} status changed to {order.Status}.",
                    OrderId = order.Id
                };

                await _notificationService.CreateAsync(notification, cancellationToken);

                // Real-time push will be handled by the Presentation layer or a hosted service.
                // At minimum, we persisted the notification above so clients can poll or receive via SignalR hub implementation in the server project.
            }
        }
    }
}
