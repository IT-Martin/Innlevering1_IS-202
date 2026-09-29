# Innlevering1

Gruppe 2 holde på å utvikle en ASP.NET Core MVC-applikasjon for å registrere med visning av ressurser på et kart.

Brukeren kan skrive inn et ressursnavn, velge en kartposisjon og registrere ressursen. Navn og koordinater vises deretter på en egen resultatside. Registrerte ressurser kan også vises samlet på kartet.

## 1. Drift

For å kjøre prosjektet gjennom Visual Studio trenger du:

- Visual Studio med støtte for .NET 10, ASP.NET.
- Docker Desktop installert på maskinen.
- Internettilgang for å hente nødvendige pakker som Leaflet.

## 2. Starte applikasjonen

1. Klon GitHub-repositoriet, eller last ned og pakk ut prosjektet.
2. Start Docker Desktop.
3. Åpne "Innlevering1.slnx" i Visual Studio.
4. Kontroller at "Innlevering1_Gruppe2" er oppstartsprosjektet.
5. Velg kjøreprofilen 'Container (Dockerfile)'.
6. Start prosjektet med den grønne startknappen.
7. Når applikasjonen har startet, åpner Visual Studio nettleseren.

> Første oppstart "kan ta litt tid", vent ca. 1 min for å se om applikasjonen starter.

### Stoppe applikasjonen

Stopp kjøringen i Visual Studio ved å trykke på "Stop Debugging" oppe til høyre.

## 3. SystemArkitektur

Applikasjonen bruker ASP.NET Core MVC (Model, View and Controller). 

**Model:** I prosjektet brukes 'RessursViewModel' til å representere en ressurs med ID, navn og koordinater. Data sendes til Controller som deretter sendes til Viewet for brukeren.

**View:** Hva slags innhold som skal vises til brukeren. Her brukes det Razor, HTML, CSS og JavaScript til blant annet registreringsskjemaet, resultatsiden og kartet.

**Controller:** Er logikken i backend, som håndterer GET og POST request fra View. Ved POST mottar controlleren ressursnavn og koordinater fra skjemaet. Den kontrollerer dataen og lagrer ressursen i Session.
                  Ved GET, for eksempel når brukeren åpner kartet, henter controlleren ressurslisten og sender den til riktig view. 

| Komponent | Ansvar |
|---|---|
| HomeController.cs | Håndterer GET og POST, kontrollerer innsendte data, leser og lagrer nye ressurser i ressurslisten |
| RessursViewModel.cs | Gir mulighet til å lage en ressurs med verdier: ID, navn og kart koordinatene |
| Register.cshtml | Viser skjemaet og kartet for valg av ressursens posisjon. |
| Result.cshtml | Viser navn og koordinater for ressursen controlleren har hentet. |
| Map.cshtml | Viser ressurslisten og oppretter kartmarkører ved hjelp av JavaScript |
| _Layout.cshtml | Inneholder felles navigasjon for nettsiden
| Program.cs | MVC, Session og ruting |
| Dockerfile | Beskriver hvordan applikasjon håndterer midlertidige lagring og generelt oppstart av applikasjon



### Lagring av data

Ressursene lagres midlertidlig i brukerens Session. Ressurslisten gjøres om til JSON og oppbevares i applikasjonens minne.

Det betyr at:

- Ressursene tilhører den aktuelle brukerøkten.
- Data kan forsvinne når økten utløper eller applikasjonen starter på nytt.
- Denne versjonen bruker ikke database.

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


## KI Bruk:

Gruppe deltakere sin bruk av KI.


Mål:
- Controller, view modell og view.
- Responsive nettsider med dynamisk innhold hentet fra web server.
- At applikasjonen håndterer GET og POST forespørsler.
- Skjema som tar data fra brukeren og at data vises i annen nettside.
- Kart i tillegg til skjemaet (punktet over) og at data hentes fra kartet og vises i en annen nettside.
- Dokumentasjon i Github om drift, system arkitektur, testing scenarier og resultater.
- Dokumentasjonen i selve koden.
- Dokumenter deres bruk av KI i prosjektet fra ide til koding i Github READ ME (bruksområder, verktøy, prompt kommandoer).

Vår bruk av AI
- Vi har brukt AI til alt.
