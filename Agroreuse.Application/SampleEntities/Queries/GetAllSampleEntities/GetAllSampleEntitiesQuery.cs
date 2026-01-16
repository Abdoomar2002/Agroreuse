using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.SampleEntities.DTOs;

namespace Agroreuse.Application.SampleEntities.Queries.GetAllSampleEntities;

/// <summary>
/// Query to get all SampleEntities.
/// </summary>
public record GetAllSampleEntitiesQuery : IQuery<IReadOnlyList<SampleEntityDto>>;
