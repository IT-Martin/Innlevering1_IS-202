# Reachout – SQL steg for steg

Reachout bruker én database. I den ligger 19 prosjekttabeller og én teknisk tabell for EF Core. Tabellenes struktur er hentet direkte fra den fungerende `reachout_database`, med primærnøkler, fremmednøkler, indekser, datatyper og databasesjekker.

Skriptene oppretter en separat database kalt `reachout_sql_oving`. De skal ikke kjøres oppå den eksisterende Reachout-databasen. De gamle Docker-databasene fra andre prosjektkopier er ikke del av denne pakken.

## 1. Velg hva dere vil gjøre

**Bare se eksisterende data:** Kjør `23_se_eksisterende_data.sql`. Den inneholder bare lesespørringer og peker på `reachout_database`. Dere trenger ikke opprette en database først.

**Bygge strukturen selv for å lære:** Kjør `00` til `21` i stigende rekkefølge. Hver fil velger øvingsdatabasen før den oppretter sin tabell. Kjør deretter `22` for å kontrollere resultatet.

**Opprette hele strukturen på én gang:** Kjør `ALT_opprett_hele_databasen.sql` i stedet for `00`–`21`. Ikke kjør begge alternativene. CREATE TABLE-filene er ment for en tom database; de gir feil hvis tabellene allerede finnes.

Pakken kopierer ikke brukerkontoer, passordhasher, personlige ressurser eller innloggingstokener. Den inkluderer faste kategorier, ferdigheter, den tekniske låseraden og korrekt migrasjonshistorikk. Det isolerte testoppsettet kan opprette eksempeldata.

## 2. Koble til databasen i Rider

Velg Database → + → Data Source → MariaDB. Fyll inn:

| Felt | Verdi |
|---|---|
| Host | `127.0.0.1` |
| Port | `3316` |
| User | `gruppe2_app` |
| Database ved lesing | `reachout_database` |
| Password | Databasepassordet dere har fått i chatten / lokal `.env` |

Trykk Test Connection og deretter OK. Utvid databasen og Tables for å se tabellene. Åpne en SQL-fil, velg riktig tilkobling og kjør filens setninger.

Brukeren `gruppe2_app` har tilgang til den eksisterende Reachout-databasen. Opprettelse av en ny øvingsdatabase krever administratorrettigheter. Alternativet i neste steg bruker Docker-containerens eksisterende administratorinnlogging og krever ikke at dere endrer passord eller utvider appbrukerens tilgang.

## 3. Kjør SQL-filene fra Docker-terminalen

Åpne en terminal i prosjektets rotmappe. Kommandoene leser filer fra `database/scripts`.

Denne kommandoen kjøres i terminalen på Mac-en eller Docker Desktops nederste zsh-terminal, ikke inne i en SQL-ledetekst:

```sh
docker compose exec -T db sh -c 'exec mariadb -uroot -p"$MARIADB_ROOT_PASSWORD"' < database/scripts/00_opprett_ovingsdatabase.sql
```

Kjør deretter samme kommando med neste filnavn. For eksempel:

```sh
docker compose exec -T db sh -c 'exec mariadb -uroot -p"$MARIADB_ROOT_PASSWORD"' < database/scripts/01_Account.sql
docker compose exec -T db sh -c 'exec mariadb -uroot -p"$MARIADB_ROOT_PASSWORD"' < database/scripts/02_PublicOrganization.sql
```

Fortsett med filene i tabellen nedenfor til og med `21_faste_grunndata.sql`. Kommandoene bruker passordet som allerede er konfigurert inne i databasecontaineren; dere skal ikke skrive eller bytte root-passordet.

Hvis dere heller vil kjøre alt samlet:

```sh
docker compose exec -T db sh -c 'exec mariadb -uroot -p"$MARIADB_ROOT_PASSWORD"' < database/scripts/ALT_opprett_hele_databasen.sql
```

## 4. Tabellenes rekkefølge og hensikt

| Fil | Hva opprettes? | Hvorfor på dette trinnet? |
|---|---|---|
| `00_opprett_ovingsdatabase.sql` | Øvingsdatabasen | Gir en egen plass for tabellene. |
| `01_Account.sql` | Kontoer, e-post, passordhash og sikkerhetsfelter | Underkontoene peker på Account. |
| `02_PublicOrganization.sql` | Offentlige organisasjoner og godkjenning | Offentlige kontoer og invitasjoner trenger denne. |
| `03_Location.sql` | Adresse/område og kartkoordinater | Ressurser og behov peker hit. |
| `04_ResourceType.sql` | Ressurskategorier | Ressurser og behov må ha en kategori. |
| `05_Skill.sql` | Ferdigheter | Brukes av frivilligprofiler. |
| `06_ProviderAccount.sql` | Leverandørkontoer | Deler primærnøkkel med Account. |
| `07_PublicActorAccount.sql` | Offentlige kontoer og rettigheter | Kobler Account til PublicOrganization. |
| `08_VolunteerProfile.sql` | Frivilligprofiler | Kobles til ProviderAccount. Unik account_id gir høyst én profil per leverandør. |
| `09_Resource.sql` | Ressurser | Peker på ProviderAccount, ResourceType og Location. |
| `10_ResourceNeed.sql` | Hjelpebehov | Peker på offentlig konto, kategori og sted. |
| `11_Availability.sql` | Tilgjengelighetsperioder | Krever nøyaktig én eier: ressurs eller frivilligprofil. Slutt må være etter start. |
| `12_VolunteerProfileSkill.sql` | Kobling mellom frivilligprofil og ferdighet | Sammensatt primærnøkkel hindrer samme ferdighet to ganger på samme profil. |
| `13_ResourceMatch.sql` | Kobling mellom ressurs og behov | Unik resource_need_id gir høyst én match per behov. |
| `14_Reservation.sql` | Reservasjoner | Unik resource_match_id gir høyst én reservasjon per match. |
| `15_VolunteerInterest.sql` | Frivilliges interesse for behov | Holdes separat fra reservasjoner. |
| `16_AccountToken.sql` | Hashverdier for engangskoder og lenker | Peker på Account. |
| `17_OrganizationInvitation.sql` | Personlige organisasjonsinvitasjoner | Peker på PublicOrganization. |
| `18_AuditEntry.sql` | Hendelseslogg | account_id er et felt, men har ingen fremmednøkkel i dagens modell. |
| `19_AppWriteLock.sql` | Teknisk låsetabell | Brukes av appen for å samordne samtidige endringer. |
| `20___EFMigrationsHistory.sql` | EF Cores migrasjonshistorikk | Gjør det mulig å registrere hvilket skjema som allerede er opprettet. |
| `21_faste_grunndata.sql` | 8 kategorier, 2 ferdigheter, låserad og 4 migrasjonsrader | Kjøres når alle tabellene finnes. |
| `22_kontroller_strukturen.sql` | Kontrollspørringer | Forventer 20 tabeller. Viser koblinger, indekser og grunndata. |
| `23_se_eksisterende_data.sql` | Lesespørringer mot dagens Reachout | Viser registrerte ressurser, behov, reservasjoner, frivillige og interesser. |

En fremmednøkkel betyr at raden det pekes på må finnes. Derfor opprettes Account før ProviderAccount, og Resource før ResourceMatch. Dere trenger ikke slå av kontrollen av fremmednøkler.

## 5. Kontroller øvingsdatabasen

```sh
docker compose exec -T db sh -c 'exec mariadb -uroot -p"$MARIADB_ROOT_PASSWORD"' < database/scripts/22_kontroller_strukturen.sql
```

Resultatet skal vise 20 tabeller: 19 prosjekttabeller og `__EFMigrationsHistory`. Det skal også ligge 8 rader i ResourceType, 2 i Skill, én låserad i AppWriteLock og 4 migrasjonsrader.

## 6. Se hva som faktisk er lagt til i Reachout

Dette kan dere gjøre med appbrukeren og det eksisterende passordet:

```sh
docker compose exec db mariadb -u gruppe2_app -p reachout_database
```

Når dere er logget inn:

```sql
SHOW TABLES;
DESCRIBE Resource;
SELECT resource_id, resource_name, availability_status FROM Resource;
SELECT resource_need_id, title, status FROM ResourceNeed;
SELECT reservation_id, reservation_status FROM Reservation;
exit;
```

`SHOW TABLES` viser tabeller. `DESCRIBE` viser felter og datatyper. `SELECT` leser innhold. Kommandoene over endrer ikke data.

## Avgrensning

Dette er skjemaet som Reachout faktisk bruker, inkludert de fire støttetabellene utover ER-diagrammet. Det er ikke en forenklet kopi av diagrammet. Opprettelsesskriptene er for en ny database; oppgradering av den gamle prosjektbasen bør gjøres med appens EF Core-migrasjoner og kontrolleres på en sikkerhetskopi først. MariaDBs DDL kan gi automatisk commit, så hele opprettelsen kan ikke behandles som én garantert reverserbar transaksjon.

Skriptene er kontrollert mot MariaDB 11.8.9. Se TESTRESULTAT.txt. Den eksisterende Reachout-databasen er ikke endret under kontrollen.
