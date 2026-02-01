using BLL.Abstrack;
using Entity.DataTransferObject.MessageDTO;
using Entity.DataTransferObject.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talky_MVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var MessageList = _messageService.GetAll().Data;
            return View(MessageList);
        }


        [HttpGet("id")]
        public IActionResult Detail(int id)
        {
            var messageGet = _messageService.Get(id).Data;
            return View(messageGet);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(MessageAddDTO message)
        {
            _messageService.Add(message);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var messageId = _messageService.Get(id).Data;
                return View(messageId);
            }

            return RedirectToAction();
        }


        [HttpPost]
        public IActionResult Edit(MessageUpdateDTO message)
        {
            _messageService.Update(message);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _messageService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
