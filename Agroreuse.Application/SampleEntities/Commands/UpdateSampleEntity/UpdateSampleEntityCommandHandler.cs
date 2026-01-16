using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Exceptions;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.SampleEntities.Commands.UpdateSampleEntity;

/// <summary>
/// Handler for UpdateSampleEntityCommand.
/// </summary>
public class UpdateSampleEntityCommandHandler : ICommandHandler<UpdateSampleEntityCommand>
{
    private readonly ISampleEntityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSampleEntityCommandHandler(
        ISampleEntityRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SampleEntity), request.Id);

        entity.Update(request.Name, request.Description);

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
