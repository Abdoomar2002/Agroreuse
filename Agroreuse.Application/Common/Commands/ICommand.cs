using MediatR;

namespace Agroreuse.Application.Common.Commands;

/// <summary>
/// Marker interface for commands.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for commands with a response.
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}
