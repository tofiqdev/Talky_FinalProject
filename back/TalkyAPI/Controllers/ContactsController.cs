using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalkyAPI.Data;
using TalkyAPI.DTOs.User;
using TalkyAPI.Models;

namespace TalkyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        // GET: api/contacts
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetMyContacts()
        {
            var userId = GetUserId();

            var contacts = await _context.Contacts
                .Where(c => c.UserId == userId)
                .Include(c => c.ContactUser)
                .Select(c => new UserDto
                {
                    Id = c.ContactUser.Id,
                    Username = c.ContactUser.Username,
                    Email = c.ContactUser.Email,
                    Avatar = c.ContactUser.Avatar,
                    Bio = c.ContactUser.Bio,
                    IsOnline = c.ContactUser.IsOnline,
                    LastSeen = c.ContactUser.LastSeen,
                    CreatedAt = c.ContactUser.CreatedAt
                })
                .ToListAsync();

            return Ok(contacts);
        }

        // POST: api/contacts/{contactUserId}
        [HttpPost("{contactUserId}")]
        public async Task<IActionResult> AddContact(int contactUserId)
        {
            var userId = GetUserId();

            // Cannot add yourself
            if (userId == contactUserId)
                return BadRequest(new { message = "Cannot add yourself as contact" });

            // Check if user exists
            var contactUser = await _context.Users.FindAsync(contactUserId);
            if (contactUser == null)
                return NotFound(new { message = "User not found" });

            // Check if already a contact
            var existingContact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ContactUserId == contactUserId);

            if (existingContact != null)
                return BadRequest(new { message = "User is already in your contacts" });

            // Check if blocked
            var isBlocked = await _context.BlockedUsers
                .AnyAsync(bu => (bu.UserId == userId && bu.BlockedUserId == contactUserId) ||
                               (bu.UserId == contactUserId && bu.BlockedUserId == userId));

            if (isBlocked)
                return BadRequest(new { message = "Cannot add blocked user as contact" });

            var contact = new Contact
            {
                UserId = userId,
                ContactUserId = contactUserId,
                AddedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact added successfully" });
        }

        // DELETE: api/contacts/{contactUserId}
        [HttpDelete("{contactUserId}")]
        public async Task<IActionResult> RemoveContact(int contactUserId)
        {
            var userId = GetUserId();

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ContactUserId == contactUserId);

            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contact removed successfully" });
        }

        // GET: api/contacts/check/{contactUserId}
        [HttpGet("check/{contactUserId}")]
        public async Task<ActionResult<bool>> CheckIsContact(int contactUserId)
        {
            var userId = GetUserId();

            var isContact = await _context.Contacts
                .AnyAsync(c => c.UserId == userId && c.ContactUserId == contactUserId);

            return Ok(isContact);
        }
    }
}
