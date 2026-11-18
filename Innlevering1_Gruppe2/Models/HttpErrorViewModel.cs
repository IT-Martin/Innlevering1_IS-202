namespace Reachout.Models;

public class HttpErrorViewModel
{
    public int StatusCode { get; set; }

    public string Title => StatusCode switch
    {
        400 => "Siden må oppdateres",
        401 => "Logg inn for å fortsette",
        403 => "Du har ikke tilgang",
        404 => "Siden finnes ikke",
        429 => "Vent litt før du prøver igjen",
        _ => "Vi kunne ikke fullføre forespørselen"
    };

    public string Message => StatusCode switch
    {
        400 => "Skjemaet kunne ikke godkjennes. Siden kan ha vært åpen under en omstart, eller innloggingen kan ha endret seg i en annen fane. Åpne siden på nytt og prøv igjen.",
        401 => "Du må være innlogget for å åpne denne siden.",
        403 => "Brukeren din har ikke tilgang til denne handlingen.",
        404 => "Lenken kan være gammel, eller innholdet er ikke tilgjengelig for brukeren din.",
        429 => "Det er sendt mange forespørsler på kort tid. Vent ett minutt og prøv igjen.",
        _ => "Prøv igjen om litt. Hvis problemet fortsetter, ta kontakt med den som drifter Reachout."
    };
}
