using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseGenerator.Common.Database.Types;

public record ArchiveEvent : ArchiveDbItem<ArchiveEvent>
{
    public required string UserName { get; set; }
    public ArchiveUser User { get; set; } = null!;
    
    public required EventType EventType { get; set; }
    public required DateTime CreationDate { get; set; }
    
    public required string? LevelId { get; set; }
    [ForeignKey(nameof(LevelId))]
    public ArchiveLevel? Level { get; set; }
    
    /* // todo: make this link properly. the id is given with that weird fucking timestamp and idfk whats going on
    public required long? CommentId { get; set; }
    [ForeignKey(nameof(CommentId))]
    public ArchiveComment? Comment { get; set; }
    */

    public override bool SamePrimaryKey(ArchiveEvent other)
    {
        return this.UserName == other.UserName && this.EventType == other.EventType &&
               this.CreationDate == other.CreationDate;
    }
}