using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database.Types;

namespace DatabaseGenerator.FromPages.Types;

public record PageComment : IArchiveItem
{
    public required string UserName { get; set; }
    public required string Text { get; set; }
    public required long Timestamp { get; set; }
    /// <summary>
    /// Genuinely don't understand what this is for. Replies? Edits? either way, i've not been able to get it to work well
    /// </summary>
    public required long? SecondTimestamp { get; set; }
    public required Guid? ArticleId { get; set; }
    public required string? ArticleName { get; set; }
    public required string? ArticleContentUrl { get; set; }
    public required string? ArticleIconUrl { get; set; }
    public required string? ArticleSummary { get; set; }
    public required string? LevelId { get; set; }
    public required string Source { get; set; }
    
    public ArchiveComment ToArchiveComment()
    {
        return new ArchiveComment
        {
            UserName = this.UserName,
            Text = this.Text,
            ArticleId = this.ArticleId,
            LevelId = this.LevelId,
            Sources = this.Source,
            Timestamp = this.Timestamp,
            SecondTimestamp = this.SecondTimestamp
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
    
    public ArchiveArticle? ToArchiveArticle()
    {
        if (this.ArticleId == null)
            return null;
        
        return new ArchiveArticle
        {
            Id = (Guid)this.ArticleId,
            Name = this.ArticleName,
            ContentUrl = this.ArticleContentUrl,
            Sources = this.Source,
            Summary = this.ArticleSummary,
            IconUrl = this.ArticleContentUrl
        };
    }
}