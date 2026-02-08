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

        public UpdateOrderCommandHandler(
            IOrderRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.Id);

            order.Update(
                request.AddressId,
                request.CategoryId,
                request.Quantity,
                request.NumberOfDays,
                request.Status);

            // Update images if provided
            if (request.ImagePaths != null)
            {
                order.ClearImages();
                var imagesToAdd = request.ImagePaths.Take(4).ToList();
                foreach (var imagePath in imagesToAdd)
                {
                    var orderImage = new OrderImage(imagePath, order.Id);
                    order.AddImage(orderImage);
                }
            }

            await _repository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
