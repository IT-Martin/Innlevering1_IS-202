<h1 align="center">Innlevering1</h1>

> Gruppe 2 holde på å utvikle en **ASP.NET Core MVC-applikasjon** for å registrere med visning av ressurser på et kart.

Brukeren kan skrive inn et ressursnavn, velge en kartposisjon og registrere ressursen. Navn og koordinater vises deretter på en egen resultatside. Registrerte ressurser kan også vises samlet på kartet.

---

## 1. Drift og oppstart

For å kjøre prosjektet trenger du:

- **Visual Studio** med støtte for **.NET 10** og **ASP.NET Core**
- **Docker Desktop**
- En tilgjengelig **MySQL/MariaDB-database**
- Internettilgang for eksterne ressurser som **Leaflet** og **OpenStreetMap**
- Database kobling med


## 1. Førstegangsoppsett

### Åpne prosjektet

1. Klon GitHub-repositoriet eller last ned prosjektet.
2. Start **Docker Desktop**.
3. Åpne `Innlevering1.slnx` i **Visual Studio**.
4. Sett `Innlevering1_Gruppe2` som **oppstartsprosjekt**.

### Opprett databasen i Docker

Kjør følgende kommando i terminalen:

```powershell
docker run --name resourcedb -p 127.0.0.1:3307:3306 -v resourcedb-data:/var/lib/mysql -e MARIADB_ROOT_PASSWORD=Password -e MARIADB_DATABASE=resourcedb -d mariadb:latest
```

Kommandoen oppretter MariaDB-containeren `resourcedb` og en database med samme navn. Databasen er tilgjengelig på port `3307` på din PC.

## Opprett bruker inni databasen:
```
CREATE USER 'beredskap_app'@'%'
IDENTIFIED BY 'SETT_INN_DET_GENERERTE_APPPASSORDET';

GRANT SELECT, INSERT, UPDATE, DELETE
ON resourcedb.*
TO 'beredskap_app'@'%';

SHOW GRANTS FOR 'beredskap_app'@'%';
```
Denne brukeren har kun tilgang til resourcedb, å ikke andre databaser i mariadb containern.

### Konfigurer tilkoblingen for migreringer

Åpne `appsettings.json` og oppdater `ConnectionStrings`. Behold de øvrige innstillingene i filen.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=resourcedb;User=beredskap_app;Password=hemmeligpassord;"
}
```

> Adressen `127.0.0.1` brukes når du oppdaterer databasen med migreringer fra Visual Studio.

### Opprett databasestrukturen

Åpne **Tools → NuGet Package Manager → Package Manager Console** i Visual Studio og kjør:

```powershell
Update-Database
```

Kommandoen bruker prosjektets eksisterende migreringer til å opprette eller oppdatere tabeller og relasjoner i databasen.

### Konfigurer tilkoblingen for applikasjonen

Når migreringene er fullført, endrer du `Server` i tilkoblingsstrengen til `host.docker.internal`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=host.docker.internal;Port=3307;Database=resourcedb;User=beredskap_app;Password=hemmeligpassord;"
}
```

---

## Opprette Konto
Brukere har mulighet til å opprette leverandør konto. Denne konto lagres i mariadb.

### Lokal demonstrasjons konto

Logge inn som offentlig konto:

- E-post: `offentlig@test.no`
- Passord: `offentligoffentligoffentlig`

## Systemarkitektur

Applikasjonen følger Model-View-Controller-arkitekturen:

- **Model:** Inneholder entiteter som representerer dataene i databasen, blant annet kontoer, ressurser, ressurstyper og lokasjoner. ViewModel-klasser brukes til å motta, validere og sende data mellom skjemaene, controllerne og visningene.
- **View:** Består av Views som presenterer brukergrensesnittet. Viewene viser skjemaer, navigasjon, ressursinformasjon, innloggingsstatus og kart. Bootstrap, CSS, JavaScript, Leaflet og Font Awesome brukes til utforming og interaktivitet.
- **Controller:** Mottar HTTP GET- og POST-forespørsler og styrer applikasjonens flyt. Controllerne validerer brukerinput, håndterer innlogging og autorisering, og koordinerer med ApplicationDbContext og ResourceRepository for å hente og lagre data. De sender deretter riktige modeller eller ViewModels til Views.


| Komponent | Ansvar |
| --- | --- |
| `Program.cs` | Konfigurerer applikasjonen, databaseforbindelse, avhengighetsinjeksjon, cookie-autentisering, autorisering, rate limiting, middleware og MVC-routing. Kjører også migreringer og eventuell startdata ved oppstart. |
| `Controllers/LoginController.cs` | Håndterer registrering, innlogging, utlogging, e-postbekreftelse og tilgangsavvisning. Bruker `AccountService` for autentisering og claims. |
| `Controllers/AccountController.cs` | Håndterer glemt passord, nytt passord og visning av brukerprofil. |
| `Controllers/ResourcesController.cs` | Håndterer registrering, redigering, arkivering, detaljer og egne ressurser. Kontrollerer innlogging og at brukeren har tilgang til ressursen. |
| `Controllers/NeedsController.cs` | Håndterer opprettelse, redigering, arkivering, detaljer og visning av ressursbehov. |
| `Controllers/RequestsController.cs` | Viser forespørsler og lar leverandører eller offentlige aktører svare på og oppdatere forespørsler. |
| `Controllers/AvailabilityController.cs` | Viser, oppretter og fjerner tilgjengelighetsperioder for ressurser og frivilligprofiler. |
| `Controllers/VolunteersController.cs` | Viser frivilligkatalogen og lar private brukere opprette, redigere og arkivere frivilligprofilen sin. |
| `Controllers/IntrestsController.cs` | Håndterer interesser og påmeldinger knyttet til frivilligoppdrag. Filnavnet følger den eksisterende skrivemåten `Intrests`. |
| `Controllers/OrganizationsController.cs` | Håndterer søknader om organisasjoner, godkjenning, invitasjoner og administrasjon av organisasjonsmedlemmer. |
| `Controllers/MapController.cs` | Henter ressurser og behov og gjør dem tilgjengelige som kartdata for kartvisningen. |
| `Controllers/ExportsController.cs` | Eksporterer tilgjengelige ressurser som CSV og viser en utskriftsvennlig oversikt. |
| `Controllers/HomeController.cs` | Viser forsiden, informasjonssiden og feilsider. |
| `DataAccess/ApplicationDbContext.cs` | Kobler applikasjonen til MariaDB gjennom Entity Framework Core og definerer entiteter, relasjoner, nøkler, validering, indekser og standarddata. |
| `DataAccess/Repositories/IResourceRepository.cs` | Definerer operasjonene repository-laget tilbyr for å hente og administrere ressurser. |
| `DataAccess/Repositories/ResourceRepository.cs` | Implementerer databaseoperasjoner for ressurser, inkludert henting av eier, lokasjon og ressurstype. |
| `Services/AccountService.cs` | Håndterer tilgangskontroll for kontoer, hashing av tokens, passordrelaterte lenker, e-post og cookie-baserte claims. |
| `Services/WorkflowService.cs` | Samler regler for ressurskoblinger, forespørsler, reservasjoner og tilgjengelighetsperioder. Bruker transaksjoner og en databaselås for å hindre samtidige konflikter. |
| `Models/Entities/*.cs` | Representerer domenedataene som lagres i databasen, blant annet kontoer, organisasjoner, ressurser, behov, reservasjoner, frivillige og ferdigheter. |
| `Models/*ViewModel.cs` | Input- og presentasjonsmodeller med validering for skjemaer og visning av data i views. |
| `Views/**/*.cshtml` | Razor Views som rendrer HTML-sidene og skjemaene i MVC-applikasjonen. |
| `wwwroot/` | Inneholder statiske filer som CSS, JavaScript, Bootstrap, Leaflet, bilder og andre frontend-ressurser. |



### Lagring av data

Ressursene lagres permanent i en MySQL-database gjennom Entity Framework Core til MariaDB i docker. Datamodellen inneholder blant annet tabeller for Account, Location, ProviderAccount, Resource og ResourceType.

Databaseforbindelsen konfigureres gjennom `ConnectionStrings:DefaultConnection`, og migreringer kjøres automatisk når applikasjonen starter.


## 4. Testing

| ID | Testscenario | Forventet resultat | Faktisk resultat | Status |
| --- | --- | --- | --- | --- |
| T1 | Logg inn med riktig e-postadresse og passord. | Brukeren blir logget inn og sendt videre til kart- eller ressurssiden. | Brukeren ble logget inn og sendt videre til kartet. | Bestått |
| T2 | Logg inn med riktig e-postadresse, men feil passord. | Innloggingen stoppes, og meldingen «Feil e-post eller passord» vises. | Innloggingen ble stoppet, og riktig feilmelding ble vist. | Bestått |
| T3 | Registrer en konto med en e-postadresse som allerede finnes. | Kontoen opprettes ikke, og brukeren får beskjed om at e-postadressen allerede er i bruk. | Kontoen ble ikke opprettet, og feilmeldingen ble vist. | Bestått |
| T4 | Registrer en ressurs, åpne «Mine ressurser» og rediger navn og beskrivelse. | De oppdaterte verdiene lagres og vises korrekt etter redigering. | Ressursen ble oppdatert, og de nye verdiene ble vist. | Bestått |
| T5 | Registrer en ressurs og slett den fra «Mine ressurser». | Ressursen fjernes fra listen og vises ikke lenger på kartet. | Ressursen ble fjernet fra både listen og kartet. | Bestått |
| T6 | Logg inn med én konto og forsøk å åpne eller redigere en ressurs som tilhører en annen konto. | Brukeren får ikke tilgang til den andre brukerens ressurs. | Tilgang til ressursen ble nektet. | Bestått |
| T7 | Logg inn, logg ut, og prøv deretter å åpne kartet eller «Mine ressurser» direkte. | Brukeren blir sendt til innloggingssiden og får ikke tilgang til beskyttede sider. | Brukeren ble sendt til innloggingssiden, og sidene var ikke tilgjengelige. | Bestått |


## Enhetstesting:

Enhetstestene ligger i `tests/Innlevering1_Gruppe2.Tests`. Krever .NET 10 SDK. Fra rotmappen kjører du dem slik:

macOS/Linux:
```bash
./run-enhetstester
```

Windows (PowerShell eller Ledetekst):
```bat
.\run-enhetstester.bat
```

Testene dekker validering av registrerings-, innloggings- og ressursmodeller, ikonvalg for ressurstyper, innlogging med ukjent e-post eller feil passord, ressursregistrering og redigering, tilgang til egne ressurser og repository-operasjoner. Databasetestene bruker en isolert EF Core-database i minnet og krever ikke en kjørende MariaDB-instans. Skriptet bruker den vanlige oppsummeringen fra `dotnet test`.

## Sikkerhetstesting

| ID | Test | Gjennomføring | Resultat | Vurdering |
| --- | --- | --- | --- | --- |
| S1 | SQL injection | Innloggingsskjemaet ble testet med ugyldig e-postadresse. Deretter ble en gyldig e-postadresse brukt sammen med `' or 1=1 --` i passordfeltet. | Ugyldig e-postadresse ble avvist av valideringen. Med gyldig e-postadresse og SQL-payload i passordfeltet ble meldingen «Feil e-post eller passord» vist. | Ingen innloggingsomgåelse ble påvist med dette forsøket. |
| S2 | Cross-Site Scripting (XSS) | Payloaden `<script>alert('XSS attack')</script>` ble testet i URL-en og ved registrering av en ny ressurs. | Ingen dialogboks ble vist under forsøkene. | Ingen XSS ble påvist med denne payloaden på de testede stedene. |
| S3 | Lagring av passord | Passordverdien som var lagret i databasen, ble undersøkt. | Passordet var lagret som en hash og ikke som lesbar klartekst. | Lagring som hash er observert. Hashalgoritme og innstillinger er ikke vurdert i denne testen. |
| S4 | Kryptering under overføring | Wireshark ble brukt til å undersøke trafikken mellom applikasjonen og databasen på port 3307. | Opptaket viste TLS 1.3-forhandling og etterfølgende kryptert trafikk. Ingen passord ble observert i klartekst. | TLS-kryptering ble bekreftet for de undersøkte databaseforbindelsene. |



## Brukervennlighetstesting

Begge testpersonene fikk samme oppgave i Beredskapssentralen: «Tenk deg at du har et aggregat du kan låne ut. Registrer det slik at andre kan se hvor det er tilgjengelig.» Vi ba dem starte på forsiden og observerte hvordan de fant registreringsskjemaet, fylte ut ressursinformasjonen og valgte en plassering på kartet. 

### Bruker test 1
Personen fikk til å legge inn ressurs. Det tok litt tid å lete på hvor på siden man kan legge til ressursen hen. Inne på registrer ressurs, fant brukeren greit ut hvordan man legger inn informasjon for å registrere aggregat.

Hva vi forbedret: Vi la til en rød knapp med teksten «Registrer ressurs» for å gjøre funksjonen lettere å finne. Vi gjorde også kartet større, slik at menyen med ressursinformasjon og knappen tar opp en mindre del av kartvisningen.

### Bruker test 2
Personen reagerte litt på det å finne knappen for å legge til ressurs, brukte litt ekstra tid. Nevnte blant annet om å bruke sin nåværende GPS for å legge til en ressurs hadde vært veldig fint.

Hva vi forbedret: Vi la til en knapp som henter brukerens nåværende GPS-posisjon. Knappen er tilgjengelig både når brukeren registrerer en ressurs og når brukeren melder et behov. Vi la også til knappen «Legg til ressurs» direkte på kartet, slik at brukeren kan starte registreringen derfra.

## Vår bruk av AI:
Dokumentasjon på vårres bruk a KI i prosjektet fra ide til koding i Github (bruksområder, verktøy, prompt kommandoer)

### Martin
Jeg har brukt KI som støtte til å forstå programmering i praksis. Leser først koden som KI svarer og prøver å forstå hva den går ut på. Hender det at jeg møter noe nye koder, funksjoner eller logikk, så ber jeg KI om å forklare hva den aktuelle kodelinjen sin funksjon er, eventuelt hvordan det henger sammen med applikasjonen. Som MVC. Samtidig gjør jeg litt research og spør LA om hjelp når det skjer.

Det meste koden jeg får eller leser er skrevet inn manuelt. Hvis jeg har skrevet tilsvarende kode tidligere eller føler at jeg forstår den godt, hender det at jeg kopierer og limer inn. Men det er veldig sjeldent. Jeg har også endret koden og tilpasset kode forslag fra KI i prosjektet.

Min læringsstrategi er mer at jeg bruker KI som hjelpe middel. Min ønske er å bearbeide det jeg lærer og huske det bedre over tid. Samtidig ha det i muskel minne. Jeg synes per i dag det er alt for lett å kopiere forslag fra KI uten å forstå dem. Målet mitt er å lære effektivt og på god måte. Samtidig å møte utfordringer og forsøke å forstå feil er viktig del av læringen min. Samtidig takle utfordringer som stopper meg opp, bruke stoppen til å søke etter feilen selv. Lese error. Å få vinne følelsen fordi jeg rette problemet. Ikke KI. Dette gir meg langsiktig motivasjon og glede i prosjektet.

Jeg bruker ChatGPT Plus, og opplever det som en nyttig læringsverktøy for meg. Det gir meg mulighet å zippe prosjektmappen, sende inn til KI. Å få den til vurdere hele prosjektet uten den endrer repo fra Git. 

Jeg har prøvd ut Copilot, men det har ikke fungert helt for meg siden det blir litt mye tull fra min side. Men det er vel smak og behag går jeg ut ifra.

### Eksempel på en prompt fra meg:

(Min prosjektmappe som ZIP.)

Ok her er prosjektmappa min. Se gjennom alt og få med deg logikken. Så få med deg hva jeg burde kommen der inne for Controller, view og model inne der.

## Ellen
Jeg har brukt ChatGPT Plus som et støtteverkøty gjennom prosjektet, hovedsakelig for å forstå teknologien vi bruker, utvikle ideer og få hjelp underveis når jeg står fast. Jeg har blant annet brukt KI til å forstå ASP.NET Core MVC, HTML/SS og hvordan Views, Controllers og ViewModels henger sammen. ChatGPT har også blitt brukt som veiledning i Git/GitHub, blant annet med branches, git status, git add, git commit og git push. 

I tillegg har jeg brukt KI til feilsøking ved å sende inn skjermbilder fra Visual Studio, terminalen og nettsiden, og til å diskutre hvordan nettsiden kan utformes. Jeg har hovedsakelig brukt KI for å forstå hva jeg gjør, ikke bare for å få en ferdig kode. Noe som har vært veldig nyttig i en slik oppgave. 

Eksempler på prompts jeg har brukt:
- "Hva er forskjellen på git commit og git push?"
- "Kan du forklare forskjellen på "a" og "li"?
- "Kan du hjelpe meg å forstå denne Git-statusen?"
- "Kan du forklare denne HTML/CSS-koden?"
 
  osv.


## Sander

Jeg har brukt ChatGPT litt underveis i prosjektet, hovedsakelig når jeg har stått fast eller trengt en forklaring på noe. Jeg har blant annet brukt det til å forstå enkelte deler av ASP.NET Core MVC og til å feilsøke når jeg har fått problemer.

Jeg har også brukt KI til å få hjelp med enkelte deler av HTML, CSS og JavaScript. Forslagene har først og fremst vært brukt som veiledning, og jeg har selv testet og tilpasset det i prosjektet.

Eksempler på prompts som eg har brukt er:

* «Kan du forklare meg denne feilmeldingen?
* «Hva gjør denne delen av koden?»
* «Hvordan fungerer Controller og View sammen i MVC?»
* «Hvorfor fungerer ikke dette?»


## Fredrik
KI-verktøy har blitt brukt som et støtteverktøy gjennom arbeidet med oppgaven. Det har hovedsakelig blitt brukt til å få hjelp med forståelse av oppgaveteksten og relevante fagbegreper. KI har også blitt brukt til å diskutere mulige løsninger knyttet til modeller og systemstruktur.

## Kornelius
Har brukt ChatGPT og Github Copilot. Bruket har hovedsakelig vært til feilsøking i kode og drøfting rundt koden. Til feilsøking er prompts jeg bruker hovedsakelig å lime inn konsollens feilmelding og output eller screenshots av relevant info, så skriver jeg hva jeg sliter med, hva jeg tror feilen er, eller forklare hva som prøves å oppnås.

## Lennart
Jeg har brukt Copilot som støtte i arbeidet for å få bedre oversikt, struktur og forståelse av hvordan prosjektet skulle bygges opp. Verktøyet hjalp med å organisere filer, forklare sammenhenger i MVC modellen og gi veiledning når jeg sto fast.
