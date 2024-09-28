using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database.Types;

namespace DatabaseGenerator.FromPages.Types;

public record PageProfile : IArchiveItem
{
    public required string Name { get; set; }
    public required Guid? IdentityId { get; set; }
    public required int? LevelsPlayed { get; set; }
    public required int? LevelsPublished { get; set; }
    public required string Source { get; set; }

    public ArchiveUser ToArchiveUser()
    {
        return new ArchiveUser
        {
            Name = this.Name,
            Sources = this.Source,
            IdentityId = this.IdentityId,
            LevelsPlayed = this.LevelsPlayed,
            LevelsPublished = this.LevelsPublished
        };
    }
}