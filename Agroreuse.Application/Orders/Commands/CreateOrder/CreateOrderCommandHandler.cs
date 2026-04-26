using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Interface;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Handler for CreateOrderCommand.
    /// </summary>
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddressRepository _addressRepository;

        public CreateOrderCommandHandler(
            IOrderRepository repository,
            IUnitOfWork unitOfWork,
            IAddressRepository addressRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _addressRepository = addressRepository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Create a new address for the order (independent from user's profile address)
            var orderAddress = new Address(
                request.Address.GovernmentId,
                request.Address.CityId,
                request.Address.Details);

            await _addressRepository.AddAsync(orderAddress, cancellationToken);

            // Update or create user's profile address with the same data (but as a separate object)
            var userProfileAddress = await _addressRepository.GetByUserIdAsync(request.SellerId, cancellationToken);

            if (userProfileAddress != null)
            {
                // Update existing profile address
                userProfileAddress.GovernmentId = request.Address.GovernmentId;
                userProfileAddress.CityId = request.Address.CityId;
                userProfileAddress.Details = request.Address.Details;
                await _addressRepository.UpdateAsync(userProfileAddress, cancellationToken);
            }
            else
            {
                // Create new profile address for the user
                var newProfileAddress = new Address(
                    request.Address.GovernmentId,
                    request.Address.CityId,
                    request.Address.Details)
                {
                    ApplicationUserId = request.SellerId
                };
                await _addressRepository.AddAsync(newProfileAddress, cancellationToken);
            }

            // Create the order with the order-specific address
            var order = new Order(
                request.SellerId,
                orderAddress.Id,
                request.CategoryId,
                request.Description,
                request.Quantity,
                request.NumberOfDays);

            // Add images (max 4)
            if (request.ImagePaths != null)
            {
                var imagesToAdd = request.ImagePaths.Take(4).ToList();
                foreach (var imagePath in imagesToAdd)
                {
                    var orderImage = new OrderImage(imagePath, order.Id);
                    order.AddImage(orderImage);
                }
            }

            await _repository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
