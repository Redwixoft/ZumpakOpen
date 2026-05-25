namespace ZumpakOpen.Models;

public class Match
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public int WinningThreshold { get; set; }
    public bool IsFinished { get; set; } = false;

    public int? WinnerId { get; set; }
    public Participant? Winner { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; } = new List<PlayerScore>();
}
