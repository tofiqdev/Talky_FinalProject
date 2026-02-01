using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.StoryViewDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryViewController : ControllerBase
    {
        private readonly IStoryViewService _storyViewService;
        private readonly IMapper _mapper;

        public StoryViewController(IStoryViewService storyViewService, IMapper mapper)
        {
            _storyViewService = storyViewService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _storyViewService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<StoryViewListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _storyViewService.GetById(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<StoryViewListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] StoryViewAddDTO storyViewAddDTO)
        {
            var result = _storyViewService.Add(storyViewAddDTO);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _storyViewService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("story/{storyId}")]
        public IActionResult GetStoryViewers(int storyId)
        {
            var result = _storyViewService.GetAll();
            if (result.IsSuccess)
            {
                var viewers = result.Data
                    .Where(sv => sv.StoryId == storyId)
                    .OrderByDescending(sv => sv.ViewedAt)
                    .ToList();
                
                var data = _mapper.Map<List<StoryViewListDTO>>(viewers);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("story/{storyId}/count")]
        public IActionResult GetViewCount(int storyId)
        {
            var result = _storyViewService.GetAll();
            if (result.IsSuccess)
            {
                var count = result.Data.Count(sv => sv.StoryId == storyId);
                return Ok(new { storyId, viewCount = count });
            }
            return BadRequest(result.Message);
        }
    }
}
