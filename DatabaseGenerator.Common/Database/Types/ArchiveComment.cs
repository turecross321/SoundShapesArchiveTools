using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Database.Types;

[PrimaryKey(nameof(Id))]
public record ArchiveComment : ArchiveDbItem<ArchiveComment>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public required string? UserName { get; set; }
    [ForeignKey(nameof(UserName))]
    public ArchiveUser? User { get; set; } = null;
    
    public required string? Text { get; set; }
    
    public required Guid? ArticleId { get; set; }
    [ForeignKey(nameof(ArticleId))]
    public ArchiveArticle? Article { get; set; } = null;

    public required string? LevelId { get; set; }
    [ForeignKey(nameof(LevelId))]
    public ArchiveLevel? Level { get; set; } = null;
    
    public required long Timestamp { get; set; }
    public required long? SecondTimestamp { get; set; }

    public override bool SamePrimaryKey(ArchiveComment other)
    {
        return this.Id == other.Id;
    }
}