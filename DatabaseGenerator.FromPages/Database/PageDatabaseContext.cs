using DatabaseGenerator.Common;
using DatabaseGenerator.Common.Database;
using DatabaseGenerator.Common.Extensions;
using NotEnoughLogs;

namespace DatabaseGenerator.FromPages.Database;

public class PageDatabaseContext : ArchiveDatabaseContext
{
    public void AddPageImport(Logger logger, PageImporter importer)
    {
        
        (int added, int merged) = this.MergeAddRange(this.Users, importer.Profiles.Select(p => p.ToArchiveUser()));
        logger.LogInfo(LogContext.PageImport, $"Added users from profiles -- {added} new, {merged} merged");
        
        
        (added, merged) = this.MergeAddRange(this.Users, importer.Levels.Select(l => l.ToArchiveUser()));
        logger.LogInfo(LogContext.PageImport, $"Added users from level list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Users, importer.LeaderboardEntries.Select(l => l.ToArchiveUser()));
        logger.LogInfo(LogContext.PageImport, $"Added users from leaderboard list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Users, importer.Events.Select(l => l.ToArchiveUser()));
        logger.LogInfo(LogContext.PageImport, $"Added users from events -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Users, importer.Comments.Select(l => l.ToArchiveUser()));
        logger.LogInfo(LogContext.PageImport, $"Added users from comments -- {added} new, {merged} merged");
        
        
        (added, merged) = this.MergeAddRange(this.Levels, importer.Levels.Select(l => l.ToArchiveLevel()));
        logger.LogInfo(LogContext.PageImport, $"Added levels from level list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Levels, importer.Events.Select(l => l.ToArchiveLevel()));
        logger.LogInfo(LogContext.PageImport, $"Added levels from event list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Levels, importer.Comments.Select(l => l.ToArchiveLevel()));
        logger.LogInfo(LogContext.PageImport, $"Added levels from event list -- {added} new, {merged} merged");
        
        
        (added, merged) = this.MergeAddRange(this.LeaderboardEntries, importer.LeaderboardEntries.Select(e => e.ToArchiveLeaderboardEntry()));
        logger.LogInfo(LogContext.PageImport, $"Added leaderboard entries from leaderboard list -- {added} new, {merged} merged");

        (added, merged) = this.MergeAddRange(this.Articles, importer.Comments.Select(c => c.ToArchiveArticle()));
        logger.LogInfo(LogContext.PageImport, $"Added news articles from comment list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Comments, importer.Comments.Select(c => c.ToArchiveComment()));
        logger.LogInfo(LogContext.PageImport, $"Added comments from comment list -- {added} new, {merged} merged");
        
        (added, merged) = this.MergeAddRange(this.Events, importer.Events.Select(e => e.ToArchiveEvent()));
        logger.LogInfo(LogContext.PageImport, $"Added events from event list -- {added} new, {merged} merged");


        SaveChanges();
    }
}