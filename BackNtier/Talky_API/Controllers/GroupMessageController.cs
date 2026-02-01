using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.GroupmessageDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMessageController : ControllerBase
    {
        private readonly IGroupMessageService _groupMessageService;
        private readonly IMapper _mapper;

        public GroupMessageController(IGroupMessageService groupMessageService, IMapper mapper)
        {
            _groupMessageService = groupMessageService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _groupMessageService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<GroupmessageListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _groupMessageService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<GroupmessageListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] GroupmessageAddDTO groupMessageAddDTO)
        {
            var result = _groupMessageService.Add(groupMessageAddDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GroupmessageUpdateDTO groupMessageUpdateDTO)
        {
            if (id != groupMessageUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = _groupMessageService.Update(groupMessageUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _groupMessageService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("group/{groupId}")]
        public IActionResult GetGroupMessages(int groupId)
        {
            var result = _groupMessageService.GetAll();
            if (result.IsSuccess)
            {
                var messages = result.Data
                    .Where(gm => gm.GroupId == groupId)
                    .OrderBy(gm => gm.SentAt)
                    .ToList();
                
                var data = _mapper.Map<List<GroupmessageListDTO>>(messages);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }
    }
}
