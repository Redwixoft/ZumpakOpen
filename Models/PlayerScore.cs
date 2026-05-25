namespace ZumpakOpen.Models;

public class PlayerScore
{
    public int Id { get; set; }

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int ParticipantId { get; set; }
    public Participant Participant { get; set; } = null!;

    public int Points { get; set; } = 0;
}
