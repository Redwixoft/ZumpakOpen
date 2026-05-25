using Microsoft.EntityFrameworkCore;
using ZumpakOpen.Models;

namespace ZumpakOpen.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<PlayerScore> PlayerScores => Set<PlayerScore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Winner)
            .WithMany(p => p.WonMatches)
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Session)
            .WithMany(s => s.Matches)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerScore>()
            .HasOne(ps => ps.Match)
            .WithMany(m => m.PlayerScores)
            .HasForeignKey(ps => ps.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerScore>()
            .HasOne(ps => ps.Participant)
            .WithMany(p => p.PlayerScores)
            .HasForeignKey(ps => ps.ParticipantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
