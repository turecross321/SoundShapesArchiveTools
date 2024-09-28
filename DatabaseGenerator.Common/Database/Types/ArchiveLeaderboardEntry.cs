using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseGenerator.Common.Database.Types;

public record ArchiveLeaderboardEntry : ArchiveDbItem<ArchiveLeaderboardEntry>
{
    public required string UserName { get; set; }
    [ForeignKey(nameof(UserName))]
    public ArchiveUser User { get; set; } = null!;
    
    public required string LevelId { get; set; }
    [ForeignKey(nameof(LevelId))]
    public ArchiveLevel Level { get; set; } = null!;
    
    public required int TotalMilliseconds { get; set; }
    public required int TotalNotes { get; set; }

    public override void Merge(ArchiveLeaderboardEntry other)
    {
        if (other.TotalNotes == this.TotalNotes)
        {
            this.TotalMilliseconds = Math.Min(this.TotalMilliseconds, other.TotalMilliseconds);
        }
        else if (other.TotalNotes > this.TotalNotes)
        {
            this.TotalNotes = other.TotalMilliseconds;
            this.TotalMilliseconds = other.TotalMilliseconds;
        }
        
        base.Merge(other);
    }

    public override bool SamePrimaryKey(ArchiveLeaderboardEntry other)
    {
        return this.UserName == other.UserName && this.LevelId == other.LevelId;
    }
}