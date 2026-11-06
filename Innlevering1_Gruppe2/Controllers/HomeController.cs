using Reachout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Reachout.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("Platform"))
        {
            return RedirectToAction("Index", "Organizations");
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Overview));
        }

        return View();
    }

    [Authorize(Roles = "Private,Public")]
    public IActionResult Overview()
    {
        return RedirectToAction("Index", "Map");
    }

    public IActionResult About() => View();

    // Gamle bokmerker peker videre til sidene som nå eier funksjonene.
    [HttpGet]
    public IActionResult Privacy() => RedirectToAction(nameof(About));

    [Authorize(Roles = "Private,Public"), HttpGet]
    public IActionResult Map() => RedirectToAction("Index", "Map");

    [Authorize(Roles = "Private"), HttpGet]
    public IActionResult MyResources(bool archived = false) => RedirectToAction("MyContributions", "Resources", new { archived });

    [Authorize(Roles = "Private"), HttpGet]
    public IActionResult Register() => RedirectToAction("Register", "Resources");

    [Authorize(Roles = "Private"), HttpGet]
    public IActionResult Edit(int id) => RedirectToAction("Edit", "Resources", new { id });

    [Authorize(Roles = "Private,Public"), HttpGet]
    public IActionResult Result(int? id) => RedirectToAction("Registered", "Resources", new { id });

    [AllowAnonymous, IgnoreAntiforgeryToken]
    public IActionResult HttpError(int statusCode)
    {
        var code = statusCode is >= 400 and <= 599 ? statusCode : 400;
        Response.StatusCode = code;
        ViewData["IsHttpError"] = true;
        return View(new HttpErrorViewModel { StatusCode = code });
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
