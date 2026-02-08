using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Exceptions;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.Orders.Commands.DeleteOrder
{
    /// <summary>
    /// Handler for DeleteOrderCommand.
    /// </summary>
    public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderCommandHandler(
            IOrderRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.Id);

            await _repository.DeleteAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
