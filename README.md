# Innlevering1

Gruppe 2 holde på å utvikle en ASP.NET Core MVC-applikasjon for å registrere med visning av ressurser på et kart.

Brukeren kan skrive inn et ressursnavn, velge en kartposisjon og registrere ressursen. Navn og koordinater vises deretter på en egen resultatside. Registrerte ressurser kan også vises samlet på kartet.

## Drift og oppstart

For å kjøre prosjektet trenger du:

- Visual Studio med støtte for .NET 10 og ASP.NET Core
- Docker Desktop
- En tilgjengelig MySQL/MariaDB-database
- Internettilgang for eksterne ressurser som Leaflet og OpenStreetMap



## 2. Starte applikasjonen

1. Klon GitHub-repositoriet eller last ned prosjektet.
2. Start Docker Desktop.
3. Åpne `Innlevering1.slnx` i Visual Studio.
4. Kontroller at `Innlevering1_Gruppe2` er oppstartsprosjektet.
5. Kontroller at databaseforbindelsen i `DefaultConnection` er riktig konfigurert.
6. Velg `Container (Dockerfile)`
7. Start prosjektet med den grønne startknappen.

> Første oppstart "kan ta litt tid", vent ca. 1 min for å se om applikasjonen starter.

### Stoppe applikasjonen

Stopp kjøringen i Visual Studio ved å trykke på "Stop Debugging" oppe til høyre.

## Systemarkitektur

Applikasjonen følger Model-View-Controller-arkitekturen:

- **Model:** Inneholder entiteter som representerer dataene i databasen, blant annet kontoer, ressurser, ressurstyper og lokasjoner. ViewModel-klasser brukes til å motta, validere og sende data mellom skjemaene, controllerne og visningene.
- **View:** Består av Views som presenterer brukergrensesnittet. Viewene viser skjemaer, navigasjon, ressursinformasjon, innloggingsstatus og kart. Bootstrap, CSS, JavaScript, Leaflet og Font Awesome brukes til utforming og interaktivitet.
- **Controller:** Mottar HTTP GET- og POST-forespørsler og styrer applikasjonens flyt. Controllerne validerer brukerinput, håndterer innlogging og autorisering, og koordinerer med ApplicationDbContext og ResourceRepository for å hente og lagre data. De sender deretter riktige modeller eller ViewModels til Views.


| Komponent | Ansvar |
| --- | --- |
| `Controllers/LoginController.cs` | Håndterer registrering, innlogging, utlogging og tilgangsavvisning. Validerer brukerdata, hasher og verifiserer passord, oppretter autentiserings-cookie og claims. |
| `Controllers/ResourceController.cs` | Håndterer kartvisning, registrering, redigering, sletting og visning av brukerens egne ressurser. Krever innlogging og kontrollerer at brukeren eier ressursen før redigering eller sletting. |
| `Controllers/HomeController.cs` | Viser forsiden, personvernsiden og feilsiden. |
| `DataAccess/ApplicationDbContext.cs` | Kobler applikasjonen til databasen gjennom Entity Framework Core. Definerer entiteter, relasjoner, nøkler, valideringsregler, indekser og standardtyper for ressurser. |
| `DataAccess/Repositories/IResourceRepository.cs` | Definerer operasjonene repository-laget må støtte for å hente, opprette, oppdatere og slette ressurser. |
| `DataAccess/Repositories/ResourceRepository.cs` | Implementerer databaseoperasjoner for ressurser ved hjelp av Entity Framework Core. Håndterer også henting av eier, lokasjon og ressurstype. |
| `Models/Entities/Account.cs` | Representerer en brukerkonto med e-post, kontaktinformasjon, passordhash, kontotype og eide ressurser. |
| `Models/Entities/Resource.cs` | Representerer en ressurs med navn, beskrivelse, eier, ressurstype, lokasjon og kontaktinformasjon. |
| `Models/Entities/Location.cs` | Lagrer adresseinformasjon og geografiske koordinater for en ressurs. |
| `Models/Entities/ResourceType.cs` | Representerer kategorier som kan brukes til å klassifisere ressurser. |
| `Models/Entities/ProviderAccount.cs` | Representerer leverandør- eller organisasjonsinformasjon. Entiteten er konfigurert i databasen, men brukes ikke aktivt i hovedflyten ennå. |
| `Models/AccountRegistrationViewModel.cs` | Inputmodell for brukerregistrering med validering av navn, telefon, e-post og passord. |
| `Models/LoginViewModel.cs` | Inputmodell for innlogging med validering av e-post og passord. |
| `Models/ResourceCreateViewModel.cs` | Inputmodell for registrering av ressurs, inkludert navn, beskrivelse, kategori og koordinater. |
| `Models/ResourceEditViewModel.cs` | Inputmodell for redigering av en eksisterende ressurs. |
| `Models/RessursViewModel.cs` | Presentasjonsmodell som brukes til å vise ressurser, ikoner og koordinater på kartet og resultatsiden. |
| `Models/ResourceTypeIcon.cs` | Kobler ressurstyper og ressursnavn til passende Font Awesome-ikoner. |



### Lagring av data

Ressursene lagres permanent i en MySQL-database gjennom Entity Framework Core. Datamodellen inneholder blant annet tabeller for Account, Location, ProviderAccount, Resource og ResourceType.

Databaseforbindelsen konfigureres gjennom `ConnectionStrings:DefaultConnection`, og migreringer kjøres automatisk når applikasjonen starter.


## 4. Testing

| ID | Testscenario | Forventet resultat | Faktisk resultat | Status |
|---|---|---|---|---|
| T1 | Skriv inn “Traktor”, velg en kartposisjon og klikk på “Registrer ressurs”. | Riktig navn og koordinater vises. | Riktig navn og koordinater ble vist. | Bestått |
| T2 | Forsøk å sende skjemaet med tomt ressursnavn. | Innsendingen stoppes, og brukeren får beskjed om at navnet må fylles ut. | Resultatet viser at brukeren får beskjed “Vennligst fyll ut dette feltet”. | Bestått. |
| T3 | Skriv inn navn uten å velge kartposisjon | Registreringsknappen er deaktivert. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T4 | Registrer to ressurser på forskjellige posisjoner | Begge vises i listen og som markører på kartet. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T5 | Oppdater resultatsiden etter registrering | Ressursnavn og koordinater forblir uendret, og ingen ekstra ressurser opprettes. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T6 | Klikk på en posisjon i registreringsskartet, og klikk deretter et annet sted. | Den gamle markøren fjernes. En markør vises på den nye posisjonen, og koordinatene oppdateres. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T7 | Registrerer “Traktor”, trykk “Se alle ressursene på kartet” og se om markøren kommer med navnet “Traktor”. | En popup viser ressursnavnet ‘Traktor’. Etter registreringen. | Kartet ble sentrert på ressursen. Da jeg klikket på markøren, viste popupen navnet “Traktor” | Ikke bestått. |

## Enhetstesting:

## Systemstesting

## Sikkerhetstesting 

## Brukervennlighetstesting





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





### Lage tabell og database i docker desktop

Ta opp terminal på Docker Desktop. 

Kopier følgende kommando: 

docker run --name resourcedb -p 127.0.0.1:3306:3306 -v resourcedb-data:/var/lib/mysql -e MARIADB_ROOT_PASSWORD=Password -e MARIADB_DATABASE=resourcedb -d mariadb:latest

Passordet er: Password (På engelsk)

Sjekk om du kan gå inn i containern:
 docker exec -it resourcedb mariadb -u root -p. Så skriv inn passordet

Inne på Visual studio prosjekt, gå inn på: appsettings.json. Å legg til Connection stringen: "Server=127.0.0.1;Port=3307;Database=resourcedb;User=root;Password=Password;"

127.0.0.1 brukes for å koble til databasen og legge til migration

Når man starter webapplikasjonen, så kan det hende man må endre til Server=host.docker.internal.

Gå inn på Package Manager Console: Skriv Update-Database. Dette lager 4 tabeller som er laget klar i visual studio kode. Som skal opprette resourcedb database med 4 tabeller.
