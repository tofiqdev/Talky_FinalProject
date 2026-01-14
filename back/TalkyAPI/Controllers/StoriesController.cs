using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalkyAPI.Data;
using TalkyAPI.DTOs;
using TalkyAPI.Models;

namespace TalkyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/stories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryDto>>> GetStories()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var now = DateTime.UtcNow;

            // Get active stories (not expired)
            var stories = await _context.Stories
                .Include(s => s.User)
                .Include(s => s.Views)
                .Where(s => s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new StoryDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Username = s.User.Username,
                    ImageUrl = s.ImageUrl,
                    Caption = s.Caption,
                    CreatedAt = s.CreatedAt,
                    ExpiresAt = s.ExpiresAt,
                    ViewCount = s.Views.Count,
                    HasViewed = s.Views.Any(v => v.UserId == currentUserId)
                })
                .ToListAsync();

            return Ok(stories);
        }

        // GET: api/stories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryDto>> GetStory(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var story = await _context.Stories
                .Include(s => s.User)
                .Include(s => s.Views)
                .Where(s => s.Id == id)
                .Select(s => new StoryDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Username = s.User.Username,
                    ImageUrl = s.ImageUrl,
                    Caption = s.Caption,
                    CreatedAt = s.CreatedAt,
                    ExpiresAt = s.ExpiresAt,
                    ViewCount = s.Views.Count,
                    HasViewed = s.Views.Any(v => v.UserId == currentUserId)
                })
                .FirstOrDefaultAsync();

            if (story == null)
                return NotFound(new { message = "Story not found" });

            return Ok(story);
        }

        // POST: api/stories
        [HttpPost]
        public async Task<ActionResult<StoryDto>> CreateStory([FromBody] CreateStoryDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var story = new Story
            {
                UserId = userId,
                ImageUrl = dto.ImageUrl,
                Caption = dto.Caption,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _context.Stories.Add(story);
            await _context.SaveChangesAsync();

            // Load user info
            var user = await _context.Users.FindAsync(userId);

            var storyDto = new StoryDto
            {
                Id = story.Id,
                UserId = story.UserId,
                Username = user?.Username ?? "",
                ImageUrl = story.ImageUrl,
                Caption = story.Caption,
                CreatedAt = story.CreatedAt,
                ExpiresAt = story.ExpiresAt,
                ViewCount = 0,
                HasViewed = false
            };

            return CreatedAtAction(nameof(GetStory), new { id = story.Id }, storyDto);
        }

        // POST: api/stories/{id}/view
        [HttpPost("{id}/view")]
        public async Task<IActionResult> ViewStory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var story = await _context.Stories.FindAsync(id);
            if (story == null)
                return NotFound(new { message = "Story not found" });

            // Check if already viewed
            var existingView = await _context.StoryViews
                .FirstOrDefaultAsync(sv => sv.StoryId == id && sv.UserId == userId);

            if (existingView != null)
                return Ok(new { message = "Already viewed" });

            // Add view
            var storyView = new StoryView
            {
                StoryId = id,
                UserId = userId,
                ViewedAt = DateTime.UtcNow
            };

            _context.StoryViews.Add(storyView);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Story viewed" });
        }

        // GET: api/stories/{id}/views
        [HttpGet("{id}/views")]
        public async Task<ActionResult<IEnumerable<StoryViewDto>>> GetStoryViews(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if story belongs to current user
            var story = await _context.Stories.FindAsync(id);
            if (story == null)
                return NotFound(new { message = "Story not found" });

            if (story.UserId != userId)
                return Forbid();

            var views = await _context.StoryViews
                .Include(sv => sv.User)
                .Where(sv => sv.StoryId == id)
                .OrderByDescending(sv => sv.ViewedAt)
                .Select(sv => new StoryViewDto
                {
                    Id = sv.Id,
                    StoryId = sv.StoryId,
                    UserId = sv.UserId,
                    Username = sv.User.Username,
                    ViewedAt = sv.ViewedAt
                })
                .ToListAsync();

            return Ok(views);
        }

        // DELETE: api/stories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var story = await _context.Stories.FindAsync(id);
            if (story == null)
                return NotFound(new { message = "Story not found" });

            if (story.UserId != userId)
                return Forbid();

            _context.Stories.Remove(story);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Story deleted" });
        }
    }
}
