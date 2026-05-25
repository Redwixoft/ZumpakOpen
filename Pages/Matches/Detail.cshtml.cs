using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Data;

namespace ZumpakOpen.Pages.Matches;

public class DetailModel(AppDbContext db) : PageModel
{
    public int MatchId { get; set; }
    public int SessionId { get; set; }
    public int MatchNumber { get; set; }
    public int WinningThreshold { get; set; }
    public bool IsFinished { get; set; }
    public int? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public List<ScoreRow> Scores { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var match = await db.Matches
            .Include(m => m.Session)
            .Include(m => m.PlayerScores)
                .ThenInclude(ps => ps.Participant)
            .Include(m => m.Winner)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (match is null) return NotFound();

        MatchId = match.Id;
        SessionId = match.SessionId;
        WinningThreshold = match.WinningThreshold;
        IsFinished = match.IsFinished;
        WinnerId = match.WinnerId;
        WinnerName = match.Winner?.Name;

        MatchNumber = await db.Matches
            .Where(m => m.SessionId == match.SessionId && m.Id <= match.Id)
            .CountAsync();

        Scores = match.PlayerScores
            .OrderBy(ps => ps.Participant.Name)
            .Select(ps => new ScoreRow(ps.ParticipantId, ps.Participant.Name, ps.Points))
            .ToList();

        return Page();
    }

    public record ScoreRow(int ParticipantId, string Name, int Points);
}
