using BLL.Abstrack;
using Entity.DataTransferObject.CallDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        private readonly ICallService _callService;
        private readonly IMapper _mapper;

        public CallController(ICallService callService, IMapper mapper)
        {
            _callService = callService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _callService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<CallListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _callService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<CallListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CallAddDTO callAddDTO)
        {
            var result = _callService.Add(callAddDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CallUpdateDTO callUpdateDTO)
        {
            if (id != callUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = _callService.Update(callUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _callService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("history/{userId}")]
        public IActionResult GetCallHistory(int userId)
        {
            var result = _callService.GetAll();
            if (result.IsSuccess)
            {
                var history = result.Data
                    .Where(c => c.CallerId == userId || c.ReceiverId == userId)
                    .OrderByDescending(c => c.StartedAt)
                    .ToList();
                
                var data = _mapper.Map<List<CallListDTO>>(history);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("missed/{userId}")]
        public IActionResult GetMissedCalls(int userId)
        {
            var result = _callService.GetAll();
            if (result.IsSuccess)
            {
                var missedCalls = result.Data
                    .Where(c => c.ReceiverId == userId && c.Status == "missed")
                    .OrderByDescending(c => c.StartedAt)
                    .ToList();
                
                var data = _mapper.Map<List<CallListDTO>>(missedCalls);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("endcall/{id}")]
        public IActionResult EndCall(int id)
        {
            var result = _callService.Get(id);
            if (result.IsSuccess)
            {
                result.Data.EndedAt = DateTime.UtcNow;
                result.Data.Status = "completed";
                result.Data.Duration = (int)(result.Data.EndedAt.Value - result.Data.StartedAt).TotalSeconds;
                
                var updateDto = _mapper.Map<CallUpdateDTO>(result.Data);
                var updateResult = _callService.Update(updateDto);
                return Ok(updateResult.Message);
            }
            return NotFound(result.Message);
        }
    }
}
