using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZumpakOpen.Data;
using ZumpakOpen.Models;

namespace ZumpakOpen.Pages.Sessions;

public class CreateModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
        Input.Date = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var names = (Input.ParticipantNames ?? Array.Empty<string>())
            .Select(n => n?.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        if (names.Count < 2)
            ModelState.AddModelError("", "Please add at least 2 participants.");

        if (!ModelState.IsValid)
            return Page();

        var session = new Session
        {
            Name = Input.Name.Trim(),
            Date = Input.Date,
            Participants = names.Select(n => new Participant { Name = n! }).ToList()
        };

        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        return RedirectToPage("/Sessions/Detail", new { id = session.Id });
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Session name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        public string[]? ParticipantNames { get; set; }
    }
}
