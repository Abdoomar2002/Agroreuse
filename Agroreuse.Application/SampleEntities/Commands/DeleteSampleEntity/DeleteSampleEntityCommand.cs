using Agroreuse.Application.Common.Commands;

namespace Agroreuse.Application.SampleEntities.Commands.DeleteSampleEntity;

/// <summary>
/// Command to delete a SampleEntity.
/// </summary>
public record DeleteSampleEntityCommand(Guid Id) : ICommand;
