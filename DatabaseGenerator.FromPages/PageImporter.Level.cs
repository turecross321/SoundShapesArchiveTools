using DatabaseGenerator.FromPages.Types;
using HtmlAgilityPack;
using NotEnoughLogs;

namespace DatabaseGenerator.FromPages;

public partial class PageImporter
{
    private void ImportFromLevelPage(Logger logger, HtmlDocument document, string filePath)
    {
        HtmlNode? contents = document.DocumentNode.SelectSingleNode("//body/div[@id='page']/div[@id='contents']");
        HtmlNode? levelDetail = contents.SelectSingleNode("./div[@id='levelDetail']");
        HtmlNode? levelLeftColumn = levelDetail.SelectSingleNode("./div[@id='levelLeftColumn']");
        HtmlNode? levelRightColumn = levelDetail.SelectSingleNode("./div[@id='levelRightColumn']");

        HtmlNode? thumbnail = levelLeftColumn.SelectSingleNode("./img");
        Guid resourceGuid = thumbnail.GetAttributeValue("src", "").GetResourceFromThumbnailSrc();

        List<HtmlNode> h1Tags = levelRightColumn.SelectNodes("./h1").ToList();
        string? levelName = h1Tags[0].InnerText;
        string? userName = h1Tags[1].SelectSingleNode("./a").InnerText;

        HtmlNodeCollection? stats = levelRightColumn.SelectSingleNode("./p[@class='meta']").SelectNodes("./strong");
        int plays = stats[0].InnerText.ToInt();
        int likes = stats[1].InnerText.ToInt();
        int averageLives = stats[2].InnerText.ToInt();

        HtmlNode? reportForm = levelRightColumn.SelectSingleNode("./form[@id='levelReportForm']/div/fieldset");
        string formattedId = reportForm.SelectSingleNode("./input[@class='object_id']").GetAttributeValue("value", "");

        PageLevel level = new PageLevel
        {
            ResourceGuid = resourceGuid,
            Name = levelName,
            Id = PageHelper.GetLevelIdFromFormatted(formattedId),
            UserName = userName,
            Plays = plays,
            Likes = likes,
            AverageLives = averageLives,
            VersionTimestamp = PageHelper.GetVersionTimestampFromFormatted(formattedId),
            Source = filePath
        };
        
        this.Levels.Add(level);
        
        HtmlNode? leaderboard = contents.SelectSingleNode("./div[@id='leftColumn']/div[@class='panel col3']/ul[@class='leaderboard']");
        
        
        if (leaderboard != null)
        {
            HtmlNodeCollection? leaderboardLis = leaderboard.SelectNodes("./li");
            foreach (HtmlNode? li in leaderboardLis)
            {
                HtmlNode? h3 = li.SelectSingleNode("./h3");

                int position = h3.InnerText.Split('.')[0].ToInt();
                userName = h3.SelectSingleNode("./a").InnerText;
                string[] scoreMetas = li.SelectSingleNode("./p[@class='meta']").InnerText.Split(new[] { ", " }, StringSplitOptions.None);

                string timeText = scoreMetas[0];
                string[] timeParts = timeText.Split(':');
                string[] secondsParts = timeParts[1].Split('.');
                int totalMilliseconds = int.Parse(timeParts[0]) * 60000 + (int.Parse(secondsParts[0]) * 1000) + int.Parse(secondsParts[1]);

                int totalNotes = scoreMetas[1].Split(' ')[0].ToInt();

                this.LeaderboardEntries.Add(new PageLeaderboardEntry
                {
                    UserName = userName,
                    LevelId = level.Id,
                    TotalMilliseconds = totalMilliseconds,
                    TotalNotes = totalNotes,
                    Position = position,
                    Source = filePath
                });
            }
        }
        
        // TODO: CHECK FOR COMMENTS
    }
}