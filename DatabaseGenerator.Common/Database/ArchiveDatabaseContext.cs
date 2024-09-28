using DatabaseGenerator.Common.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Database;

public class ArchiveDatabaseContext : DbContext
{
    public DbSet<ArchiveUser> Users { get; set; }
    public DbSet<ArchiveLevel> Levels { get; set; }
    public DbSet<ArchiveEvent> Events { get; set; }
    public DbSet<ArchiveComment> Comments { get; set; }
    public DbSet<ArchiveArticle> Articles { get; set; }
    public DbSet<ArchiveLeaderboardEntry> LeaderboardEntries { get; set; }

    private string DbPath { get; }
    

    public ArchiveDatabaseContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        string path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "archive.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchiveLeaderboardEntry>()
            .HasKey(e => new { e.UserName, e.LevelId });

        modelBuilder.Entity<ArchiveEvent>()
            .HasKey(e => new { e.EventType, e.UserName, e.CreationDate });
        
        base.OnModelCreating(modelBuilder);
    }
}