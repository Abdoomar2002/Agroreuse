using MediatR;

namespace Agroreuse.Application.Common.Queries;

/// <summary>
/// Base interface for query handlers.
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
