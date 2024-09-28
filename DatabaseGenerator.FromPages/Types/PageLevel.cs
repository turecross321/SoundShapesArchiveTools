using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database.Types;

namespace DatabaseGenerator.FromPages.Types;

public record PageLevel : IArchiveItem
{
    public required Guid ResourceGuid { get; set; }
    public required string Name { get; set; }
    public required string Id { get; set; }

    public required string? UserName { get; set; }

    public required int? Plays { get; set; }
    public required int? Likes { get; set; }
    public required int? AverageLives { get; set; }

    public required long? VersionTimestamp { get; set; }
    public required string Source { get; set; }
    
    public ArchiveUser? ToArchiveUser()
    {
        if (this.UserName == null)
            return null;
        
        return new ArchiveUser
        {
            Name = this.UserName,
            IdentityId = null,
            LevelsPlayed = null,
            LevelsPublished = null,
            Sources = this.Source
        };

    }

    public ArchiveLevel ToArchiveLevel()
    {
        return new ArchiveLevel
        {
            UserName = this.UserName,
            Name = this.Name,
            Id = this.Id,
            Sources = this.Source,
            ResourceGuid = this.ResourceGuid,
            Plays = this.Plays,
            Likes = this.Likes,
            AverageLives = this.AverageLives,
            VersionTimestamp = this.VersionTimestamp
        };
    }
}