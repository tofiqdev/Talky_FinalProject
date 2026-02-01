using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.ContactDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;

        public ContactController(IContactService contactService, IMapper mapper)
        {
            _contactService = contactService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _contactService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<ContactListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _contactService.GetById(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<ContactListDTO>(result.Data);
                return Ok(data);
            }
            return NotFound(result.Message);
        }

        [HttpPost]
        public IActionResult Add([FromBody] ContactAddDTO contactAddDTO)
        {
            var contact = _mapper.Map<Contact>(contactAddDTO);
            var result = _contactService.Add(contact);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ContactUpdateDTO contactUpdateDTO)
        {
            if (id != contactUpdateDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            var contact = _mapper.Map<Contact>(contactUpdateDTO);
            var result = _contactService.Update(contact);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _contactService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            return NotFound(result.Message);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserContacts(int userId)
        {
            var result = _contactService.GetAll();
            if (result.IsSuccess)
            {
                var contacts = result.Data
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.AddedAt)
                    .ToList();
                
                var data = _mapper.Map<List<ContactListDTO>>(contacts);
                return Ok(data);
            }
            return BadRequest(result.Message);
        }
    }
}
