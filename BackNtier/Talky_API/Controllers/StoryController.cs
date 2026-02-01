using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.StoryDTO;
using Entity.DataTransferObject.StoryViewDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;
        private readonly IStoryViewService _storyViewService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public StoriesController(
            IStoryService storyService,
            IStoryViewService storyViewService,
            IContactService contactService,
            IUserService userService,
            IMapper mapper)
        {
            _storyService = storyService;
            _storyViewService = storyViewService;
            _contactService = contactService;
            _userService = userService;
            _mapper = mapper;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return int.Parse(userIdClaim!.Value);
        }

        // GET: api/stories
        [HttpGet]
        public IActionResult GetStories()
        {
            var currentUserId = GetUserId();
            var now = DateTime.UtcNow;

            // Get contact user IDs
            var contactsResult = _contactService.GetAll();
            var contactUserIds = contactsResult.IsSuccess
                ? contactsResult.Data.Where(c => c.UserId == currentUserId).Select(c => c.ContactUserId).ToList()
                : new List<int>();

            // Get active stories (not expired) - only from contacts and self
            var result = _storyService.GetAll();
            if (result.IsSuccess)
            {
                var stories = result.Data
                    .Where(s => s.ExpiresAt > now && (contactUserIds.Contains(s.UserId) || s.UserId == currentUserId))
                    .OrderByDescending(s => s.CreatedAt)
                    .ToList();
                
                var data = _mapper.Map<List<StoryListDTO>>(stories);
                
                // Add view count and hasViewed info
                var viewsResult = _storyViewService.GetAll();
                if (viewsResult.IsSuccess)
                {
                    foreach (var story in data)
                    {
                        var storyViews = viewsResult.Data.Where(sv => sv.StoryId == story.Id).ToList();
                        story.ViewCount = storyViews.Count;
                        story.HasViewed = storyViews.Any(sv => sv.UserId == currentUserId);
                    }
                }
                
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        // GET: api/stories/{id}
        [HttpGet("{id}")]
        public IActionResult GetStory(int id)
        {
            var currentUserId = GetUserId();
            
            var result = _storyService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<StoryListDTO>(result.Data);
                
                // Add view count and hasViewed info
                var viewsResult = _storyViewService.GetAll();
                if (viewsResult.IsSuccess)
                {
                    var storyViews = viewsResult.Data.Where(sv => sv.StoryId == id).ToList();
                    data.ViewCount = storyViews.Count;
                    data.HasViewed = storyViews.Any(sv => sv.UserId == currentUserId);
                }
                
                return Ok(data);
            }
            return NotFound(new { message = result.Message });
        }

        // POST: api/stories
        [HttpPost]
        public IActionResult CreateStory([FromBody] StoryAddDTO storyAddDTO)
        {
            var userId = GetUserId();
            storyAddDTO.UserId = userId;
            storyAddDTO.CreatedAt = DateTime.UtcNow;
            storyAddDTO.ExpiresAt = DateTime.UtcNow.AddHours(24);

            var result = _storyService.Add(storyAddDTO);
            
            if (result.IsSuccess)
            {
                var data = _mapper.Map<StoryListDTO>(result.Data);
                return CreatedAtAction(nameof(GetStory), new { id = data.Id }, data);
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/stories/{id}/view
        [HttpPost("{id}/view")]
        public IActionResult ViewStory(int id)
        {
            var userId = GetUserId();
            
            var storyResult = _storyService.Get(id);
            if (!storyResult.IsSuccess)
                return NotFound(new { message = "Story not found" });

            // Check if already viewed
            var viewsResult = _storyViewService.GetAll();
            if (viewsResult.IsSuccess)
            {
                var existingView = viewsResult.Data.FirstOrDefault(sv => sv.StoryId == id && sv.UserId == userId);
                if (existingView != null)
                    return Ok(new { message = "Already viewed" });
            }

            // Add view
            var storyViewDto = new StoryViewAddDTO
            {
                StoryId = id,
                UserId = userId
            };

            var result = _storyViewService.Add(storyViewDto);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Story viewed" });
            }
            return BadRequest(new { message = result.Message });
        }

        // GET: api/stories/{id}/views
        [HttpGet("{id}/views")]
        public IActionResult GetStoryViews(int id)
        {
            var userId = GetUserId();
            
            // Check if story belongs to current user
            var storyResult = _storyService.Get(id);
            if (!storyResult.IsSuccess)
                return NotFound(new { message = "Story not found" });

            if (storyResult.Data.UserId != userId)
                return Forbid();

            var viewsResult = _storyViewService.GetAll();
            if (viewsResult.IsSuccess)
            {
                var views = viewsResult.Data
                    .Where(sv => sv.StoryId == id)
                    .OrderByDescending(sv => sv.ViewedAt)
                    .ToList();
                
                var data = _mapper.Map<List<StoryViewListDTO>>(views);
                return Ok(data);
            }
            return BadRequest(new { message = viewsResult.Message });
        }

        // DELETE: api/stories/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteStory(int id)
        {
            var userId = GetUserId();
            
            var storyResult = _storyService.Get(id);
            if (!storyResult.IsSuccess)
                return NotFound(new { message = "Story not found" });

            if (storyResult.Data.UserId != userId)
                return Forbid();

            var result = _storyService.Delete(id);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Story deleted" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}
