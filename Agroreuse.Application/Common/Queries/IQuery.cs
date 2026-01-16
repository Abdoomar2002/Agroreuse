using MediatR;

namespace Agroreuse.Application.Common.Queries;

/// <summary>
/// Marker interface for queries.
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}
