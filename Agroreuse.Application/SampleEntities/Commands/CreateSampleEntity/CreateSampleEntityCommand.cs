using Agroreuse.Application.Common.Commands;

namespace Agroreuse.Application.SampleEntities.Commands.CreateSampleEntity;

/// <summary>
/// Command to create a new SampleEntity.
/// </summary>
public record CreateSampleEntityCommand(
    string Name,
    string Description) : ICommand<Guid>;
