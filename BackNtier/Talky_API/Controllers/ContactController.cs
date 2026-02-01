using BLL.Abstrack;
using Entity.DataTransferObject.ContactDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IUserService _userService;

        public ContactController(IContactService contactService, IUserService userService)
        {
            _contactService = contactService;
            _userService = userService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return int.Parse(userIdClaim!.Value);
        }

        // GET: api/contact - Get my contacts
        [HttpGet]
        public IActionResult GetMyContacts()
        {
            var userId = GetUserId();
            
            var result = _contactService.GetAll();
            if (result.IsSuccess)
            {
                var myContacts = result.Data
                    .Where(c => c.UserId == userId)
                    .ToList();
                
                return Ok(myContacts);
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/contact - Add contact
        [HttpPost]
        public IActionResult AddContact([FromBody] int contactUserId)
        {
            var userId = GetUserId();
            
            // Check if contact already exists
            var existingContacts = _contactService.GetAll();
            if (existingContacts.IsSuccess)
            {
                var exists = existingContacts.Data.Any(c => 
                    c.UserId == userId && c.ContactUserId == contactUserId);
                
                if (exists)
                {
                    return BadRequest(new { message = "Contact already exists" });
                }
            }
            
            // Check if user exists
            var userResult = _userService.Get(contactUserId);
            if (!userResult.IsSuccess)
            {
                return NotFound(new { message = "User not found" });
            }

            var contactAddDto = new ContactAddDTO
            {
                UserId = userId,
                ContactUserId = contactUserId
            };
            
            var result = _contactService.Add(contactAddDto);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = "Contact added successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        // DELETE: api/contact/{contactUserId} - Remove contact
        [HttpDelete("{contactUserId}")]
        public IActionResult RemoveContact(int contactUserId)
        {
            var userId = GetUserId();
            
            var contacts = _contactService.GetAll();
            if (!contacts.IsSuccess)
            {
                return BadRequest(new { message = contacts.Message });
            }

            var contact = contacts.Data.FirstOrDefault(c => 
                c.UserId == userId && c.ContactUserId == contactUserId);
            
            if (contact == null)
            {
                return NotFound(new { message = "Contact not found" });
            }

            var result = _contactService.Delete(contact.Id);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = "Contact removed successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        // GET: api/contact/check/{contactUserId} - Check if user is in contacts
        [HttpGet("check/{contactUserId}")]
        public IActionResult CheckContact(int contactUserId)
        {
            var userId = GetUserId();
            
            var contacts = _contactService.GetAll();
            if (contacts.IsSuccess)
            {
                var isContact = contacts.Data.Any(c => 
                    c.UserId == userId && c.ContactUserId == contactUserId);
                
                return Ok(new { isContact });
            }
            return BadRequest(new { message = contacts.Message });
        }
    }
}
