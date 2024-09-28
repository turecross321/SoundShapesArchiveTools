using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Database.Types;

[PrimaryKey(nameof(Id))]
public record ArchiveLevel : ArchiveDbItem<ArchiveLevel>
{
    public required string Id { get; set; }
    public required Guid? ResourceGuid { get; set; }
    public required string? Name { get; set; }

    public required string? UserName { get; set; }
    public ArchiveUser? User { get; set; }

    public ICollection<ArchiveLeaderboardEntry> LeaderboardEntries { get; set; } = null!;
    public ICollection<ArchiveLevel> Comments { get; set; } = null!;

    public required int? Plays { get; set; }
    public required int? Likes { get; set; }
    public required int? AverageLives { get; set; }

    public required long? VersionTimestamp { get; set; }

    public override void Merge(ArchiveLevel other)
    {
        this.UserName ??= other.UserName;
        this.Plays = MathHelper.Max(this.Plays, other.Plays);
        this.Likes = MathHelper.Max(this.Likes, other.Likes);
        this.AverageLives = MathHelper.Max(this.AverageLives, other.AverageLives);
        this.VersionTimestamp ??= other.VersionTimestamp;
        
        base.Merge(other);
    }

    public override bool SamePrimaryKey(ArchiveLevel other)
    {
        return this.Id == other.Id; // todo: resource guid 
    }
}