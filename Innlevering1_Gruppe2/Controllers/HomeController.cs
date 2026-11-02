using Innlevering1_Gruppe2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Innlevering1_Gruppe2.Controllers;

public class HomeController : Controller
{
    //Nøkkel som brukes til å lagre og hente ressurslisten i Session
    private const string RessurserSessionKey = "Ressurser";

    //Henter ressurslisten fra brukerens Session
    //Gjør JSON-teksten om til ressursobjekter , eller returnerer en tom liste
    private List <RessursViewModel> HentRessurser()
    {
        var json = HttpContext.Session.GetString(RessurserSessionKey);

        return json is null ? new List<RessursViewModel>() : JsonSerializer.Deserialize<List<RessursViewModel>>(json) ?? new List<RessursViewModel>();
    }

    // Viser forsiden med valg av brukerinngang.
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // ENDRET: Åpner innloggingssiden og sender med hvilken brukerrolle som er valgt på forsiden.
    [HttpGet]
    public IActionResult Login(string? role)
    {
        ViewData["Role"] = role;
        return View();
    }

    // Åpner siden for opprettelse av ny brukerkonto.
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //Kart med GET Request, sender ressurslisten til kartvisning

    [HttpGet]
    public IActionResult Map()
    {
        return View(HentRessurser());
    }

    [HttpGet] //Endret navn fra Register til RegisterResource for å skille ressursregistrering fra brukerregistrering
    public IActionResult RegisterResource()
    {
        return View();
    }

    //Brukern registrere data med ressurs navn, og koordinater som er valgt på kartet.
    //Endret navn fra Register til RegisterResource slik at POST-metoden samsvarer med GET-metoden, og for ressursregistrering
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegisterResource(
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


        // Legger den nye ressursen til i listen, slik at tidligere registrering beholdes
        var ressurser = HentRessurser();
        ressurser.Add(ressurs);

        HttpContext.Session.SetString(RessurserSessionKey, JsonSerializer.Serialize(ressurser));

        return RedirectToAction(
            nameof(Result),
            new { id = ressurs.Id });

    }

    //Finner ressursen med den oppgitte ID-en og viser den på resultatsiden.
    //Hvis ressursen ikke finnes i brukerens Session, videresendes brukeren til kartet. 
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