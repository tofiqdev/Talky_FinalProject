using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalkyAPI.DTOs.Call;
using TalkyAPI.Services.Interfaces;

namespace TalkyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CallsController : ControllerBase
    {
        private readonly ICallService _callService;

        public CallsController(ICallService callService)
        {
            _callService = callService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalls()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var calls = await _callService.GetUserCalls(userId);
            return Ok(calls);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCall([FromBody] CreateCallDto createCallDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var callerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(callerIdClaim) || !int.TryParse(callerIdClaim, out int callerId))
                return Unauthorized();

            var call = await _callService.CreateCall(
                callerId,
                createCallDto.ReceiverId,
                createCallDto.CallType,
                createCallDto.Status,
                createCallDto.Duration
            );

            if (call == null)
                return BadRequest(new { message = "Failed to create call" });

            return CreatedAtAction(nameof(GetCalls), new { id = call.Id }, call);
        }
    }
}
