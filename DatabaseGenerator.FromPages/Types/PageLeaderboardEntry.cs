using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database.Types;

namespace DatabaseGenerator.FromPages.Types;

public record PageLeaderboardEntry : IArchiveItem
{
    public required string UserName { get; set; }
    public required string LevelId { get; set; }
    public required int TotalMilliseconds { get; set; }
    public required int TotalNotes { get; set; }
    public required int Position { get; set; }
    public required string Source { get; set; }
    public ArchiveLeaderboardEntry ToArchiveLeaderboardEntry()
    {
        return new ArchiveLeaderboardEntry
        {
            UserName = this.UserName,
            LevelId = this.LevelId,
            TotalMilliseconds = this.TotalMilliseconds,
            TotalNotes = this.TotalNotes,
            Sources = this.Source
        };

    }

    public ArchiveUser ToArchiveUser() // TODO NOT USED?
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