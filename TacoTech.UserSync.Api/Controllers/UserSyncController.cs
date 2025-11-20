using MediatR;
using Microsoft.AspNetCore.Mvc;
using TacoTech.UserSync.Application.Users.Commands;
using TacoTech.UserSync.Application.Users.DTOs;
using TacoTech.UserSync.Application.Users.Queries;
using TacoTech.UserSync.Application.Users.Summaries;

namespace TacoTech.UserSync.Api.Controllers
{
    /// <summary>
    /// API controller exposing endpoints to trigger user synchronization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserSyncController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSyncController"/> class.
        /// </summary>
        /// <param name="mediator">MediatR instance used to dispatch commands.</param>
        public UserSyncController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Triggers a user synchronization operation against the remote API and local SQLite database.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The synchronization summary.</returns>
        /// <response code="200">Returns the sync summary JSON.</response>
        [HttpPost("users")]
        [ProducesResponseType(typeof(SyncSummary), StatusCodes.Status200OK)]
        public async Task<ActionResult<SyncSummary>> SyncUsers(CancellationToken ct)
        {
            var summary = await _mediator.Send(new SyncUsersCommand(), ct);
            return Ok(summary);
        }

        /// <summary>
        /// Gets all users currently stored in the local SQLite database.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of local users.</returns>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IReadOnlyCollection<LocalUserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<LocalUserDto>>> GetUsers(CancellationToken ct)
        {
            var users = await _mediator.Send(new GetUsersQuery(), ct);
            return Ok(users);
        }
    }
}
