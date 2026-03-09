using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogAPI.Application.Commands.RegisterGame;
using CatalogAPI.Application.Commands.PlaceOrder;
using CatalogAPI.Application.Queries.GetAllGames;
using CatalogAPI.Application.Queries.GetGamesByUser;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController(IMediator mediator) : ControllerBase
{
    /// <summary>Registers a new game (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterGameCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Gets paginated list of games</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string name = "",
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllGamesQuery(name, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Gets all games owned by a user</summary>
    [HttpGet("library/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLibrary(Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGamesByUserQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Initiates a game purchase (publishes OrderPlacedEvent)</summary>
    [HttpPost("purchase")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Purchase([FromBody] PlaceOrderCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Accepted(new { OrderId = result.Data, Message = "Order placed. Payment is being processed." });
    }
}
