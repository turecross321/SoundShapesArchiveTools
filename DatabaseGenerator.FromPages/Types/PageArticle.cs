using DatabaseGenerator.Common;

namespace DatabaseGenerator.FromPages.Types;

public record PageArticle : IArchiveItem
{
    public required string Source { get; set; }
}