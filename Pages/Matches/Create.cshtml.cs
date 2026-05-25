using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Data;
using ZumpakOpen.Models;

namespace ZumpakOpen.Pages.Matches;

public class CreateModel(AppDbContext db) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int SessionId { get; set; }

    [BindProperty]
    [Required]
    [Range(1, 100, ErrorMessage = "Must be between 1 and 100.")]
    public int WinningThreshold { get; set; } = 13;

    public async Task<IActionResult> OnGetAsync(int sessionId)
    {
        var exists = await db.Sessions.AnyAsync(s => s.Id == sessionId);
        if (!exists) return NotFound();
        SessionId = sessionId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var participants = await db.Participants
            .Where(p => p.SessionId == SessionId)
            .ToListAsync();

        if (participants.Count == 0)
        {
            ModelState.AddModelError("", "Session has no participants.");
            return Page();
        }

        var match = new Match
        {
            SessionId = SessionId,
            WinningThreshold = WinningThreshold,
            PlayerScores = participants
                .Select(p => new PlayerScore { ParticipantId = p.Id, Points = 0 })
                .ToList()
        };

        db.Matches.Add(match);
        await db.SaveChangesAsync();

        return RedirectToPage("/Matches/Detail", new { id = match.Id });
    }
}
