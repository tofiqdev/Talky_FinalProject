using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.BlockedUserDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockedUserController : ControllerBase
    {
        private readonly IBlockedUserService _blockedUserService;
        private readonly IMapper _mapper;

        public BlockedUserController(IBlockedUserService blockedUserService, IMapper mapper)
        {
            _blockedUserService = blockedUserService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _blockedUserService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<BlockedUserListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _blockedUserService.GetById(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<BlockedUserListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] BlockedUserAddDTO blockedUserAddDTO)
        {
            var blockedUser = _mapper.Map<BlockedUser>(blockedUserAddDTO);
            var result = _blockedUserService.Add(blockedUser);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = blockedUser.Id }, blockedUser);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BlockedUserUpdateDTO blockedUserUpdateDTO)
        {
            if (id != blockedUserUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            var blockedUser = _mapper.Map<BlockedUser>(blockedUserUpdateDTO);
            var result = _blockedUserService.Update(blockedUser);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _blockedUserService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetBlockedUsers(int userId)
        {
            var result = _blockedUserService.GetAll();
            if (result.IsSuccess)
            {
                var blockedUsers = result.Data
                    .Where(bu => bu.UserId == userId)
                    .OrderByDescending(bu => bu.BlockedAt)
                    .ToList();
                
                var data = _mapper.Map<List<BlockedUserListDTO>>(blockedUsers);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("check/{userId}/{blockedUserId}")]
        public IActionResult CheckIfBlocked(int userId, int blockedUserId)
        {
            var result = _blockedUserService.GetAll();
            if (result.IsSuccess)
            {
                var isBlocked = result.Data.Any(bu => bu.UserId == userId && bu.BlockedUserId == blockedUserId);
                return Ok(new { isBlocked });
            }
            return BadRequest(result.Message);
        }
    }
}
