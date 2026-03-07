using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Exceptions;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
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
                var statusMessage = GetStatusMessage(order.Status);
                var notification = new Agroreuse.Domain.Entities.Notification
                {
                    RecipientId = order.SellerId,
                    Title = "تحديث حالة الطلب",
                    Message = statusMessage,
                    OrderId = order.Id
                };

                await _notificationService.CreateAndSendAsync(notification, cancellationToken);
            }
        }

        private static string GetStatusMessage(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "طلبك قيد الانتظار للمراجعة",
                OrderStatus.Approved => "تمت الموافقة على طلبك",
                OrderStatus.Rejected => "تم رفض طلبك",
                OrderStatus.InProgress => "طلبك قيد التنفيذ الآن",
                OrderStatus.Completed => "تم اكتمال طلبك بنجاح",
                OrderStatus.Cancelled => "تم إلغاء طلبك",
                _ => "تم تحديث حالة طلبك"
            };
        }
    }
}
