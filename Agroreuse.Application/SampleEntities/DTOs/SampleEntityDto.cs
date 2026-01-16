namespace Agroreuse.Application.SampleEntities.DTOs;

/// <summary>
/// Data transfer object for SampleEntity.
/// </summary>
public record SampleEntityDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
