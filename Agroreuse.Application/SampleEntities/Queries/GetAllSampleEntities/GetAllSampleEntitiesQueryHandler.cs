using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.SampleEntities.DTOs;
using Agroreuse.Domain.Repositories;

namespace Agroreuse.Application.SampleEntities.Queries.GetAllSampleEntities;

/// <summary>
/// Handler for GetAllSampleEntitiesQuery.
/// </summary>
public class GetAllSampleEntitiesQueryHandler : IQueryHandler<GetAllSampleEntitiesQuery, IReadOnlyList<SampleEntityDto>>
{
    private readonly ISampleEntityRepository _repository;

    public GetAllSampleEntitiesQueryHandler(ISampleEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SampleEntityDto>> Handle(
        GetAllSampleEntitiesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);

        return entities.Select(entity => new SampleEntityDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt)).ToList();
    }
}
