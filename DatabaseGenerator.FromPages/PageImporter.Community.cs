using DatabaseGenerator.FromPages.Types;
using HtmlAgilityPack;
using NotEnoughLogs;

namespace DatabaseGenerator.FromPages;

public partial class PageImporter
{
    private void ImportFromCommunityPage(Logger logger, HtmlDocument document, string filePath)
    {
        HtmlNode? levelsGrid = document.DocumentNode
            .SelectSingleNode("//body/div[@id='page']/div[@id='contents']/div[@id='mainColumn']/div[@class='panel']/ul[@class='levelsGrid']");

        HtmlNodeCollection? levelLis = levelsGrid.SelectNodes("./li");

        foreach (HtmlNode? li in levelLis)
        {
            HtmlNode? levelThumbnail = li.SelectSingleNode("./a");
            string levelId =
                levelThumbnail.GetAttributeValue("href", "").Split("/").Last();
            Guid resourceGuid = levelThumbnail.SelectSingleNode("./img").GetAttributeValue("src", "")
                .GetResourceFromThumbnailSrc();

            string levelName = li.SelectSingleNode("./h3").InnerText;
            HtmlNode? meta = li.SelectSingleNode("./p");
            HtmlNode? authorTag = meta.SelectSingleNode("./a");
            string? authorName = authorTag?.InnerText;
            
            HtmlNodeCollection? stats = meta.SelectNodes("./strong");
            int plays = stats[0].InnerText.ToInt();
            int likes = stats[1].InnerText.ToInt();
            
            this.Levels.Add(new PageLevel
            {
                ResourceGuid = resourceGuid,
                Name = levelName,
                Id = levelId,
                UserName = authorName,
                Plays = plays,
                Likes = likes,
                AverageLives = null,
                VersionTimestamp = null,
                Source = filePath
            });
        }
    }
}