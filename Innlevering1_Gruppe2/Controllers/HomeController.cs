using System.Diagnostics;
using Innlevering1_Gruppe2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Innlevering1_Gruppe2.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}