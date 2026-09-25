using Microsoft.AspNetCore.Mvc;
using Innlevering1_Gruppe2.Models.ViewModels;

namespace Innlevering1_Gruppe2.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Username == "admin" && model.Password == "1234")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Feil brukernavn eller passord");
            return View(model);
        }
    }
}