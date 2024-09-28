using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database.Types;

namespace DatabaseGenerator.FromPages.Types;

public record PageEvent : IArchiveItem
{
    public required string UserName { get; set; } 
    public required EventType EventType { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    
    public required string? LevelId { get; set; }
    public required long? CommentId { get; set; }
    public required string Source { get; set; }
    
    public ArchiveEvent ToArchiveEvent()
    {
        return new ArchiveEvent
        {
            UserName = this.UserName,
            EventType = this.EventType,
            Sources = this.Source,
            CreationDate = this.CreationDate.UtcDateTime,
            LevelId = this.LevelId,
        };

    }
    
    public ArchiveLevel? ToArchiveLevel()
    {
        if (this.LevelId == null)
            return null;
        
        return new ArchiveLevel
        {
            Id = this.LevelId,
            ResourceGuid = null,
            Name = null,
            Plays = null,
            Likes = null,
            AverageLives = null,
            VersionTimestamp = null,
            Sources = this.Source,
            UserName = null
        };

    }

    public ArchiveUser ToArchiveUser()
    {
        return new ArchiveUser
        {
            Name = this.UserName,
            IdentityId = null,
            LevelsPlayed = null,
            LevelsPublished = null,
            Sources = this.Source
        };
    }
}