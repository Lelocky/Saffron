using Microsoft.AspNetCore.Mvc;
using Spice.API.V1.ViewModels;

namespace Spice.API.Controllers
{
    [Route("v{version:apiVersion}/user")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get()
        {
            try
            {
                List<User> userList = new List<User>
                {
                    new User
                    {
                        Id = Guid.NewGuid(),
                        CreatedAtUtc = DateTime.UtcNow,
                        DiscordId = "RandomDiscordId"
                    },
                        new User
                    {
                        Id = Guid.NewGuid(),
                        CreatedAtUtc = DateTime.UtcNow,
                        DiscordId = "RandomDiscordId"
                    }
                };

                return Ok(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting users.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Details(Guid id)
        {
            if (id.Equals(Guid.Empty)) { return BadRequest(); }

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow,
                    DiscordId = "RandomDiscordId"
                };

                if (user == null) { return NotFound(); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("discorduser/{discordId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Details(string discordId)
        {
            if (string.IsNullOrEmpty(discordId)) { return BadRequest(); }

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow,
                    DiscordId = discordId
                };

                if (user == null) { return NotFound(); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Create([FromBody] User userToCreate)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        [HttpPost("edit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Edit(Guid id,[FromBody] User updateValue)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        [HttpPost("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Delete(Guid id)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return new BadRequestResult();
            }
        }
    }
}
