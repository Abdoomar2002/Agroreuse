using Agroreuse.Application.SampleEntities.Commands.CreateSampleEntity;
using Agroreuse.Application.SampleEntities.Commands.DeleteSampleEntity;
using Agroreuse.Application.SampleEntities.Commands.UpdateSampleEntity;
using Agroreuse.Application.SampleEntities.DTOs;
using Agroreuse.Application.SampleEntities.Queries.GetAllSampleEntities;
using Agroreuse.Application.SampleEntities.Queries.GetSampleEntityById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agroreuse.Server.Controllers;

/// <summary>
/// API controller for SampleEntity operations demonstrating DDD/CQRS patterns.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SampleEntitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SampleEntitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all sample entities.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SampleEntityDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllSampleEntitiesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a sample entity by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SampleEntityDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSampleEntityByIdQuery(id), cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new sample entity.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateSampleEntityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSampleEntityCommand(request.Name, request.Description);
        var id = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Update an existing sample entity.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSampleEntityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSampleEntityCommand(id, request.Name, request.Description);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a sample entity.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSampleEntityCommand(id), cancellationToken);
        return NoContent();
    }
}

/// <summary>
/// Request model for creating a sample entity.
/// </summary>
public record CreateSampleEntityRequest(string Name, string Description);

/// <summary>
/// Request model for updating a sample entity.
/// </summary>
public record UpdateSampleEntityRequest(string Name, string Description);
