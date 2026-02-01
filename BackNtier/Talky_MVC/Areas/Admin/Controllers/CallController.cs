
using BLL.Abstrack;
using Entities.DataTransferObject.CategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class CallController : Controller
    {
        private readonly ICallService _callrService;
        public CallController(ICallService callService)
        {
            _callrService = callService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var CallList = _callrService.GetAll().Data;
            return View(CallList);
        }


        [HttpGet("id")]
        public IActionResult Detail(int id)
        {
            var callGet = _callrService.Get(id).Data;
            return View(callGet);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(CallAddDTO call)
        {
            _callrService.Add(call);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var callId = _callrService.Get(id).Data;
                return View(callId);
            }

            return RedirectToAction();
        }


        [HttpPost]
        public IActionResult Edit(CallUpdateDTO call)
        {
            _callrService.Update(call);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _callrService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
