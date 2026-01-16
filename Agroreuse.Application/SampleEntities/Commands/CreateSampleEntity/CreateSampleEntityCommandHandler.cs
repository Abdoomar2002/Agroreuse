using Agroreuse.Application.Common.Commands;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.SampleEntities.Commands.CreateSampleEntity;

/// <summary>
/// Handler for CreateSampleEntityCommand.
/// </summary>
public class CreateSampleEntityCommandHandler : ICommandHandler<CreateSampleEntityCommand, Guid>
{
    private readonly ISampleEntityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSampleEntityCommandHandler(
        ISampleEntityRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = SampleEntity.Create(request.Name, request.Description);

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
