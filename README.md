# Innlevering1

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




| ID | Testscenario | Forventet resultat | Faktisk resultat | Status |
|---|---|---|---|---|
| T1 | Skriv inn “Traktor”, velg en kartposisjon og klikk på “Registrer ressurs”. | Riktig navn og koordinater vises. | Riktig navn og koordinater ble vist. | Bestått |
| T2 | Forsøk å sende skjemaet med tomt ressursnavn. | Innsendingen stoppes, og brukeren får beskjed om at navnet må fylles ut. | Resultatet viser at brukeren får beskjed “Vennligst fyll ut dette feltet”. | Bestått. |
| T3 | Skriv inn navn uten å velge kartposisjon | Registreringsknappen er deaktivert. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T4 | Registrer to ressurser på forskjellige posisjoner | Begge vises i listen og som markører på kartet. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T5 | Oppdater resultatsiden etter registrering | Ressursnavn og koordinater forblir uendret, og ingen ekstra ressurser opprettes. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T6 | Klikk på en posisjon i registreringsskartet, og klikk deretter et annet sted. | Den gamle markøren fjernes. En markør vises på den nye posisjonen, og koordinatene oppdateres. | Faktisk resultat viser til å stemme med forventet resultat. | Bestått. |
| T7 | Registrerer “Traktor”, trykk “Se alle ressursene på kartet” og se om markøren kommer med navnet “Traktor”. | En popup viser ressursnavnet ‘Traktor’. Etter registreringen. | Kartet ble sentrert på ressursen. Da jeg klikket på markøren, viste popupen navnet “Traktor” | Ikke bestått. |
