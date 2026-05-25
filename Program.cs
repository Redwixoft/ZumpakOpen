using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Data;
using ZumpakOpen.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Add points to a player in a match
app.MapPost("/api/matches/{matchId}/scores/{participantId}/add", async (
    int matchId,
    int participantId,
    PointsRequest req,
    AppDbContext db) =>
{
    if (req.Points != 1 && req.Points != 2)
        return Results.BadRequest("Points must be 1 or 2.");

    var match = await db.Matches
        .Include(m => m.PlayerScores)
            .ThenInclude(ps => ps.Participant)
        .FirstOrDefaultAsync(m => m.Id == matchId);

    if (match is null) return Results.NotFound();
    if (match.IsFinished) return Results.BadRequest("Match is already finished.");

    var score = match.PlayerScores.FirstOrDefault(ps => ps.ParticipantId == participantId);
    if (score is null) return Results.NotFound();

    score.Points += req.Points;

    if (score.Points >= match.WinningThreshold)
    {
        match.IsFinished = true;
        match.WinnerId = participantId;
    }

    await db.SaveChangesAsync();

    var winnerName = match.PlayerScores
        .FirstOrDefault(ps => ps.ParticipantId == match.WinnerId)?.Participant.Name;

    return Results.Ok(new
    {
        match.IsFinished,
        match.WinnerId,
        WinnerName = winnerName,
        Scores = match.PlayerScores.Select(ps => new
        {
            ps.ParticipantId,
            ps.Participant.Name,
            ps.Points
        })
    });
});

app.Run();

record PointsRequest(int Points);
