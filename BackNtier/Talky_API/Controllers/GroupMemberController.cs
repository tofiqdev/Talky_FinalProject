using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.GroupMenmberDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IMapper _mapper;

        public GroupMemberController(IGroupMemberService groupMemberService, IMapper mapper)
        {
            _groupMemberService = groupMemberService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _groupMemberService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<GroupMemberListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _groupMemberService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<GroupMemberListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] GroupMemberAddDTO groupMemberAddDTO)
        {
            var result = _groupMemberService.Add(groupMemberAddDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GroupMemberUpdateDTO groupMemberUpdateDTO)
        {
            if (id != groupMemberUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = _groupMemberService.Update(groupMemberUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _groupMemberService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("group/{groupId}")]
        public IActionResult GetGroupMembers(int groupId)
        {
            var result = _groupMemberService.GetAll();
            if (result.IsSuccess)
            {
                var members = result.Data
                    .Where(gm => gm.GroupId == groupId)
                    .OrderByDescending(gm => gm.JoinedAt)
                    .ToList();
                
                var data = _mapper.Map<List<GroupMemberListDTO>>(members);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserMemberships(int userId)
        {
            var result = _groupMemberService.GetAll();
            if (result.IsSuccess)
            {
                var memberships = result.Data
                    .Where(gm => gm.UserId == userId)
                    .OrderByDescending(gm => gm.JoinedAt)
                    .ToList();
                
                var data = _mapper.Map<List<GroupMemberListDTO>>(memberships);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }
    }
}
