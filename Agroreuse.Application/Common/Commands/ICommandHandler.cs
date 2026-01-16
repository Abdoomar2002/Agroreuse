using MediatR;

namespace Agroreuse.Application.Common.Commands;

/// <summary>
/// Base interface for command handlers.
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// Base interface for command handlers with a response.
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
