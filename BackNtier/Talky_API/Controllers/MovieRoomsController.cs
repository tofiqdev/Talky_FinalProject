using BLL.Abstrack;
using Entity.DataTransferObject.MovieRoomDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovieRoomsController : ControllerBase
    {
        private readonly IMovieRoomService _movieRoomService;
        private readonly IMovieRoomMessageService _messageService;

        public MovieRoomsController(
            IMovieRoomService movieRoomService,
            IMovieRoomMessageService messageService)
        {
            _movieRoomService = movieRoomService;
            _messageService = messageService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return int.Parse(userIdClaim!.Value);
        }

        // GET: api/movierooms
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _movieRoomService.GetAll();
            if (result.IsSuccess)
                return Ok(result.Data);
            
            return BadRequest(new { message = result.Message });
        }

        // GET: api/movierooms/active
        [HttpGet("active")]
        public IActionResult GetActiveRooms()
        {
            var result = _movieRoomService.GetActiveRooms();
            if (result.IsSuccess)
                return Ok(result.Data);
            
            return BadRequest(new { message = result.Message });
        }

        // GET: api/movierooms/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _movieRoomService.Get(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            
            return NotFound(new { message = result.Message });
        }

        // POST: api/movierooms
        [HttpPost]
        public IActionResult Create([FromBody] MovieRoomAddDTO dto)
        {
            dto.CreatedById = GetUserId();
            var result = _movieRoomService.Add(dto);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // PUT: api/movierooms/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MovieRoomUpdateDTO dto)
        {
            dto.Id = id;
            var result = _movieRoomService.Update(dto);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // DELETE: api/movierooms/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _movieRoomService.Delete(id);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // POST: api/movierooms/{id}/join
        [HttpPost("{id}/join")]
        public IActionResult JoinRoom(int id)
        {
            var userId = GetUserId();
            var result = _movieRoomService.JoinRoom(id, userId);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // POST: api/movierooms/{id}/leave
        [HttpPost("{id}/leave")]
        public IActionResult LeaveRoom(int id)
        {
            var userId = GetUserId();
            var result = _movieRoomService.LeaveRoom(id, userId);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // PUT: api/movierooms/{id}/playback
        [HttpPut("{id}/playback")]
        public IActionResult UpdatePlayback(int id, [FromBody] PlaybackStateDTO dto)
        {
            var result = _movieRoomService.UpdatePlaybackState(id, dto.CurrentTime, dto.IsPlaying);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        // GET: api/movierooms/{id}/messages
        [HttpGet("{id}/messages")]
        public IActionResult GetMessages(int id)
        {
            var result = _messageService.GetRoomMessages(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            
            return BadRequest(new { message = result.Message });
        }

        // POST: api/movierooms/{id}/messages
        [HttpPost("{id}/messages")]
        public IActionResult SendMessage(int id, [FromBody] MovieRoomMessageAddDTO dto)
        {
            dto.MovieRoomId = id;
            var userId = GetUserId();
            var result = _messageService.Add(dto, userId);
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }
    }

    public class PlaybackStateDTO
    {
        public double CurrentTime { get; set; }
        public bool IsPlaying { get; set; }
    }
}
