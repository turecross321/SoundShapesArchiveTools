using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Database.Types;

[PrimaryKey(nameof(Id))]
public record ArchiveArticle : ArchiveDbItem<ArchiveArticle>
{
    public required Guid Id { get; set; }
    public required string? Name { get; set; }
    public required string? ContentUrl { get; set; }
    public required string? Summary { get; set; }
    public required string? IconUrl { get; set; }
    public ICollection<ArchiveLevel> Comments { get; set; } = null!;
    public override void Merge(ArchiveArticle other)
    {
        this.Name ??= other.Name;
        this.ContentUrl ??= other.ContentUrl;
        this.Summary ??= other.Summary;
        this.IconUrl ??= other.IconUrl;
        
        base.Merge(other);
    }

    public override bool SamePrimaryKey(ArchiveArticle other)
    {
        return this.Id == other.Id;
    }
}