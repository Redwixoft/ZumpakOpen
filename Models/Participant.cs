namespace ZumpakOpen.Models;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public ICollection<Match> WonMatches { get; set; } = new List<Match>();
    public ICollection<PlayerScore> PlayerScores { get; set; } = new List<PlayerScore>();
}
