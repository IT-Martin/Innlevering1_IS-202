using Innlevering1_Gruppe2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Innlevering1_Gruppe2.Controllers;

public class HomeController : Controller
{

    private const string RessurserSessionKey = "Ressurser";

    private List <RessursViewModel> HentRessurser()
    {
        var json = HttpContext.Session.GetString(RessurserSessionKey);

        return json is null ? new List<RessursViewModel>() : JsonSerializer.Deserialize<List<RessursViewModel>>(json) ?? new List<RessursViewModel>();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //Kartet

    [HttpGet]
    public IActionResult Map()
    {
        return View(HentRessurser());
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(
        string RessursNavn,
        double? latitude,
        double? longitude)
    {
        if (string.IsNullOrWhiteSpace(RessursNavn))
        {
            ViewData["ErrorMessage"] =
                "Skriv inn navnet på ressursen.";
            return View();
        }

        if (latitude is null || longitude is null)
        {
            ViewData["ErrorMessage"] =
                "Velg en posisjon på kartet.";
            return View();
        }

        var ressurs = new RessursViewModel
        {
            RessursNavn = RessursNavn,
            Latitude = latitude.Value,
            Longitude = longitude.Value
        };


        // Legg til i listen, slik at tidligere registreringer beholdes.
        var ressurser = HentRessurser();
        ressurser.Add(ressurs);

        HttpContext.Session.SetString(RessurserSessionKey, JsonSerializer.Serialize(ressurser));

        return RedirectToAction(
            nameof(Result),
            new { id = ressurs.Id });

    }
        [HttpGet]
        public IActionResult Result(Guid? id)
        {
            var ressurs = HentRessurser().FirstOrDefault(r => r.Id == id);


            if (ressurs is null)
            {
                return RedirectToAction(nameof(Map));
            }
            return View(ressurs);
        }


    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}