using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Database.Types;

[PrimaryKey(nameof(Name))]
public record ArchiveUser : ArchiveDbItem<ArchiveUser>
{
    [Key]
    public required string Name { get; set; }

    [Key]
    public required Guid? IdentityId { get; set; }
    public required int? LevelsPlayed { get; set; }
    public required int? LevelsPublished { get; set; }

    public override void Merge(ArchiveUser other)
    {
        this.IdentityId ??= other.IdentityId;
        this.LevelsPlayed = MathHelper.Max(this.LevelsPlayed, other.LevelsPlayed);
        this.LevelsPublished = MathHelper.Max(this.LevelsPublished, other.LevelsPublished);
        
        base.Merge(other);
    }

    public override bool SamePrimaryKey(ArchiveUser other)
    {
        return this.Name == other.Name;
    }
}