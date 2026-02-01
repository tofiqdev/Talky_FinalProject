
using BLL.Abstrack;
using Entity.DataTransferObject.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var UserList = _userService.GetAll().Data;
            return View(UserList);
        }


        [HttpGet("id")]
        public IActionResult Detail(int id)
        {
            var userGet = _userService.Get(id).Data;
            return View(userGet);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(UserAddDTO user)
        {
            _userService.Add(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var userId = _userService.Get(id).Data;
                return View(userId);
            }

            return RedirectToAction();
        }


        [HttpPost]
        public IActionResult Edit(UserUpdateDTO user)
        {
            _userService.Update(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
