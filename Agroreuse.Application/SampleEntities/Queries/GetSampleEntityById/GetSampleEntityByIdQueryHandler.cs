using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.SampleEntities.DTOs;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.SampleEntities.Queries.GetSampleEntityById;

/// <summary>
/// Handler for GetSampleEntityByIdQuery.
/// </summary>
public class GetSampleEntityByIdQueryHandler : IQueryHandler<GetSampleEntityByIdQuery, SampleEntityDto?>
{
    private readonly ISampleEntityRepository _repository;

    public GetSampleEntityByIdQueryHandler(ISampleEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<SampleEntityDto?> Handle(GetSampleEntityByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return null;

        return new SampleEntityDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
