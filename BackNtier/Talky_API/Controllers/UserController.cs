using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _userService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<UserListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _userService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<UserListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(new { message = result.Message });
        }

        [HttpGet("username/{username}")]
        public IActionResult GetByUsername(string username)
        {
            var result = _userService.GetAll();
            if (result.IsSuccess)
            {
                var user = result.Data.FirstOrDefault(u => 
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound(new { message = "User not found" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var result = _userService.GetAll();
            if (result.IsSuccess)
            {
                var users = result.Data.Where(u => 
                    u.Username.Contains(q, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                
                var data = _mapper.Map<List<UserListDTO>>(users);
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserAddDTO userAddDTO)
        {
            var result = _userService.Add(userAddDTO);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            if (id != userUpdateDTO.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = _userService.Update(userUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("status")]
        public IActionResult UpdateStatus([FromBody] bool isOnline)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = _userService.Get(userId);
            if (result.IsSuccess)
            {
                var updateDto = new UserUpdateDTO
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name,
                    Username = result.Data.Username,
                    Email = result.Data.Email,
                    Avatar = result.Data.Avatar,
                    Bio = result.Data.Bio,
                    IsOnline = isOnline,
                    LastSeen = DateTime.UtcNow
                };
                var updateResult = _userService.Update(updateDto);
                
                if (updateResult.IsSuccess)
                {
                    return NoContent();
                }
                return BadRequest(new { message = updateResult.Message });
            }
            return NotFound(new { message = result.Message });
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UserUpdateDTO userUpdateDTO)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            if (userId != userUpdateDTO.Id)
            {
                return Forbid();
            }

            var result = _userService.Update(userUpdateDTO);
            
            if (result.IsSuccess)
            {
                // Return updated user data
                var updatedUser = _userService.Get(userId);
                if (updatedUser.IsSuccess)
                {
                    var userData = _mapper.Map<UserListDTO>(updatedUser.Data);
                    return Ok(userData);
                }
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("profile-picture")]
        public IActionResult UpdateProfilePicture([FromBody] UpdateAvatarRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = _userService.Get(userId);
            if (result.IsSuccess)
            {
                var updateDto = new UserUpdateDTO
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name,
                    Username = result.Data.Username,
                    Email = result.Data.Email,
                    Avatar = request.Avatar,
                    Bio = result.Data.Bio,
                    IsOnline = result.Data.IsOnline,
                    LastSeen = result.Data.LastSeen
                };
                var updateResult = _userService.Update(updateDto);
                
                if (updateResult.IsSuccess)
                {
                    // Return updated user data
                    var updatedUser = _userService.Get(userId);
                    if (updatedUser.IsSuccess)
                    {
                        var userData = _mapper.Map<UserListDTO>(updatedUser.Data);
                        return Ok(userData);
                    }
                    return Ok(new { message = "Profile picture updated successfully" });
                }
                return BadRequest(new { message = updateResult.Message });
            }
            return NotFound(new { message = result.Message });
        }

        public class UpdateAvatarRequest
        {
            public string Avatar { get; set; } = string.Empty;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _userService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return NotFound(new { message = result.Message });
        }
    }
}
