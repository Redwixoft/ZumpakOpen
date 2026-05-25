# Žumpak open

Mobile-first petanque session tracker. Tracks sessions, participants, matches, and scores.

## Stack

- .NET 8, ASP.NET Core Razor Pages, Minimal API
- EF Core 8 + PostgreSQL (Npgsql)
- Bootstrap 5 (local, from scaffold)
- Vanilla JS for live scoring (no page reload)

## Running locally

```
dotnet run
```

Migrations run automatically on startup. Before first run, update the connection string in `appsettings.json`.

## Database setup

```
dotnet ef migrations add <Name>
dotnet ef database update
```

Run from the project root (`ZumpakOpen/`).

## Project structure

```
Data/           EF Core DbContext
Models/         Entity classes (Session, Participant, Match, PlayerScore)
Pages/
  Index         Session list (homepage)
  Sessions/     Create + Detail pages
  Matches/      Create + Detail (scoring) pages
```

## Key architecture notes

- `Program.cs` hosts both Razor Pages and one Minimal API endpoint: `POST /api/matches/{matchId}/scores/{participantId}/add`
- `PlayerScore` rows are pre-created for all session participants when a match is created — the scoring endpoint only ever updates, never inserts
- Wins are derived at query time (`Matches.Count(m => m.WinnerId == p.Id)`) — not stored
- `Match.WinnerId` FK uses `OnDelete(SetNull)` and `PlayerScore → Participant` uses `OnDelete(NoAction)` to avoid EF cascade cycles
