using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Data;
using ZumpakOpen.Models;

namespace ZumpakOpen.Pages.Sessions;

public class DetailModel(AppDbContext db) : PageModel
{
    public Session Session { get; set; } = null!;
    public List<ParticipantRow> Participants { get; set; } = new();
    public List<MatchRow> Matches { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var session = await db.Sessions
            .Include(s => s.Participants)
            .Include(s => s.Matches)
                .ThenInclude(m => m.Winner)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session is null) return NotFound();

        Session = session;

        Participants = session.Participants
            .Select(p => new ParticipantRow(
                p.Id,
                p.Name,
                session.Matches.Count(m => m.WinnerId == p.Id)))
            .OrderByDescending(p => p.Wins)
            .ThenBy(p => p.Name)
            .ToList();

        Matches = session.Matches
            .Select((m, i) => new MatchRow(m.Id, i + 1, m.WinningThreshold, m.IsFinished, m.Winner?.Name))
            .ToList();

        return Page();
    }

    public record ParticipantRow(int Id, string Name, int Wins);
    public record MatchRow(int Id, int MatchNumber, int WinningThreshold, bool IsFinished, string? WinnerName);
}
