using Agroreuse.Application.Common.Commands;

namespace Agroreuse.Application.SampleEntities.Commands.UpdateSampleEntity;

/// <summary>
/// Command to update an existing SampleEntity.
/// </summary>
public record UpdateSampleEntityCommand(
    Guid Id,
    string Name,
    string Description) : ICommand;
