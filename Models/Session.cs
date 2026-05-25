namespace ZumpakOpen.Models;

public class Session
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
