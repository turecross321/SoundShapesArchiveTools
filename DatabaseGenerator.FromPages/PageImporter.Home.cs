using DatabaseGenerator.Common;
using DatabaseGenerator.FromPages.Types;
using HtmlAgilityPack;
using NotEnoughLogs;

namespace DatabaseGenerator.FromPages;

public partial class PageImporter
{
    private void ImportFromHomePage(Logger logger, HtmlDocument document, string filePath)
    {
        HtmlNodeCollection? levelLis = document.DocumentNode
            .SelectSingleNode("//body/div[@id='page']/div[@id='contents']/div[@id='leftColumn']/div[@class='panel col3']/ul[@class='thumbList']")
            ?.SelectNodes("li");

        if (levelLis == null)
        {
            logger.LogWarning(LogContext.PageImport, "Home page is missing levels. Skipping");
            return;
        }
        
        foreach (HtmlNode? li in levelLis)
        {
            HtmlNode? levelThumbnail = li.SelectSingleNode("./a");
            string levelId =
                levelThumbnail.GetAttributeValue("href", "").Split("/").Last();;
            Guid resourceGuid = levelThumbnail.SelectSingleNode("./img").GetAttributeValue("src", "")
                .GetResourceFromThumbnailSrc();

            string levelName = li.SelectSingleNode("./h3").InnerText;
            HtmlNode? meta = li.SelectSingleNode("./p");
            HtmlNode? authorTag = meta.SelectSingleNode("./a");
            string? authorName = authorTag?.InnerText;
            
            this.Levels.Add(new PageLevel
            {
                ResourceGuid = resourceGuid,
                Name = levelName,
                Id = levelId,
                UserName = authorName,
                Plays = null,
                Likes = null,
                AverageLives = null,
                VersionTimestamp = null,
                Source = filePath
            });
        }
    }
}