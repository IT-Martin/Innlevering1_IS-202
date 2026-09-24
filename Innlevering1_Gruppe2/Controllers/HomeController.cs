using Innlevering1_Gruppe2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Innlevering1_Gruppe2.Controllers
{
    public class HomeController : Controller
    {


        private readonly Database _database;
        public HomeController(Database database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy() //KOMMENTAR
        {
            return View();
        }

        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                await _database.TestConnectionAsync();

                return Content("MariaDB-tilkoblingen fungerer.");
            }
            catch (Exception exception)
            {
                return Content($"Tilkobling til MariaDB feilet: {exception.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
