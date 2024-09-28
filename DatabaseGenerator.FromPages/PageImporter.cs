using DatabaseGenerator.Common;
using DatabaseGenerator.FromPages.Types;
using HtmlAgilityPack;
using NotEnoughLogs;
using static DatabaseGenerator.FromPages.PageHelper;

namespace DatabaseGenerator.FromPages;

public partial class PageImporter : IImporter
{
    public List<PageProfile> Profiles { get; } = [];
    public List<PageLevel> Levels { get; } = [];
    public List<PageEvent> Events { get; } = [];
    public List<PageComment> Comments { get; } = []; 
    public List<PageArticle> Articles { get; } = [];
    public List<PageLeaderboardEntry> LeaderboardEntries { get; } = [];
    
    public void Import(Logger logger, string path)
    {
        string[] files = Directory.GetFiles(path);

        foreach (string filePath in files)
        {
            string content = File.ReadAllText(filePath);
            if (!content.Contains("<html"))
            {
                logger.LogWarning(LogContext.PageImport, $"[{filePath}] Not html. Skipping...");
                continue;
            }

            HtmlDocument document = new();
            document.LoadHtml(content);

            foreach (HtmlParseError? error in document.ParseErrors)
            {
                logger.LogWarning(LogContext.PageImport,
                    $"[{filePath}] HTML Parse Error: {error}. Will continue regardless...");
            }

            PageType? pageType = GetPageType(document);
            if (pageType == null)
            {
                logger.LogWarning(LogContext.PageImport,
                    $"[{filePath}] Unable to gather page type. Skipping...");
                continue;
            }

            switch (pageType)
            {
                case PageType.HomePage:
                    ImportFromHomePage(logger, document, filePath);
                    break;
                case PageType.CommunityPage:
                    ImportFromCommunityPage(logger, document, filePath);
                    break;
                case PageType.LevelPage:
                    ImportFromLevelPage(logger, document, filePath);
                    break;
                case PageType.ProfilePage:
                    ImportFromProfilePage(logger, document, filePath);
                    break;
            }

            logger.LogInfo(LogContext.PageImport, $"[{filePath}] Importing {pageType.ToString()}");
        }
    }
}