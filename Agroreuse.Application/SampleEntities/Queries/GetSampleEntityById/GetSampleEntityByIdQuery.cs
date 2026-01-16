using Agroreuse.Application.Common.Queries;
using Agroreuse.Application.SampleEntities.DTOs;

namespace Agroreuse.Application.SampleEntities.Queries.GetSampleEntityById;

/// <summary>
/// Query to get a SampleEntity by its ID.
/// </summary>
public record GetSampleEntityByIdQuery(Guid Id) : IQuery<SampleEntityDto?>;
