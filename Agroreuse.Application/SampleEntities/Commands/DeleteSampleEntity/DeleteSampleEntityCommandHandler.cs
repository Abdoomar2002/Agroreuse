using Agroreuse.Application.Common.Commands;
using Agroreuse.Application.Common.Exceptions;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.SampleEntities.Commands.DeleteSampleEntity;

/// <summary>
/// Handler for DeleteSampleEntityCommand.
/// </summary>
public class DeleteSampleEntityCommandHandler : ICommandHandler<DeleteSampleEntityCommand>
{
    private readonly ISampleEntityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSampleEntityCommandHandler(
        ISampleEntityRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SampleEntity), request.Id);

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
