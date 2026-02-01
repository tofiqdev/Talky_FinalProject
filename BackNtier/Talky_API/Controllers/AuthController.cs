using AutoMapper;
using BLL.Abstrack;
using Core.Helpers;
using Entity.DataTransferObject.AuthDTO;
using Entity.DataTransferObject.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IUserService userService, IMapper mapper, JwtHelper jwtHelper)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDTO)
        {
            // Check if user already exists
            var existingUsers = _userService.GetAll();
            if (existingUsers.IsSuccess)
            {
                var userExists = existingUsers.Data.Any(u => 
                    u.Email == registerDTO.Email || u.Username == registerDTO.Username);
                
                if (userExists)
                {
                    return BadRequest(new { message = "Kullanıcı adı veya email zaten kullanılıyor." });
                }
            }

            // Hash password
            var hashedPassword = PasswordHelper.HashPassword(registerDTO.Password);

            // Create user DTO
            var userAddDto = new UserAddDTO
            {
                Name = registerDTO.Username, // Use username as name
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                Password = hashedPassword,
                Avatar = registerDTO.Avatar,
                Bio = registerDTO.Bio
            };
            
            var result = _userService.Add(userAddDto);
            
            if (result.IsSuccess)
            {
                // Get the created user
                var users = _userService.GetAll();
                if (users.IsSuccess)
                {
                    var createdUser = users.Data.FirstOrDefault(u => u.Email == registerDTO.Email);
                    if (createdUser != null)
                    {
                        // Generate JWT token
                        var token = _jwtHelper.GenerateToken(createdUser.Id, createdUser.Email, createdUser.Username);

                        // Update user to online
                        var updateDto = new UserUpdateDTO
                        {
                            Id = createdUser.Id,
                            Name = createdUser.Name,
                            Username = createdUser.Username,
                            Email = createdUser.Email,
                            Avatar = createdUser.Avatar,
                            Bio = createdUser.Bio,
                            IsOnline = true,
                            LastSeen = DateTime.UtcNow
                        };
                        _userService.Update(updateDto);

                        var authResponse = new AuthResponseDTO
                        {
                            Token = token,
                            User = createdUser
                        };

                        return Ok(authResponse);
                    }
                }
            }
            
            return BadRequest(new { message = result.Message });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var result = _userService.GetAll();
            
            if (result.IsSuccess)
            {
                var user = result.Data.FirstOrDefault(u => 
                    u.Email == loginDTO.EmailOrUsername || u.Username == loginDTO.EmailOrUsername);

                if (user == null)
                {
                    return Unauthorized(new { message = "Kullanıcı bulunamadı." });
                }

                // Verify password
                if (!PasswordHelper.VerifyPassword(loginDTO.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Şifre hatalı." });
                }

                // Generate JWT token
                var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Username);

                // Update user to online
                var updateDto = new UserUpdateDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Bio = user.Bio,
                    IsOnline = true,
                    LastSeen = DateTime.UtcNow
                };
                _userService.Update(updateDto);

                var authResponse = new AuthResponseDTO
                {
                    Token = token,
                    User = user
                };

                return Ok(authResponse);
            }
            
            return BadRequest(new { message = result.Message });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = _userService.Get(userId);
            
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            
            return NotFound(new { message = "User not found" });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            
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
                    IsOnline = false,
                    LastSeen = DateTime.UtcNow
                };
                _userService.Update(updateDto);
                
                return Ok(new { message = "Çıkış başarılı." });
            }
            
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }
    }
}
