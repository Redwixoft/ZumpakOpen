using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Data;

namespace ZumpakOpen.Pages;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<SessionSummary> Sessions { get; set; } = new();

    public async Task OnGetAsync()
    {
        Sessions = await db.Sessions
            .OrderByDescending(s => s.Date)
            .Select(s => new SessionSummary(s.Id, s.Name, s.Date, s.Matches.Count))
            .ToListAsync();
    }

    public record SessionSummary(int Id, string Name, DateTime Date, int MatchCount);
}
