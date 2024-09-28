using DatabaseGenerator.Common;
using DatabaseGenerator.FromPages.Types;
using HtmlAgilityPack;
using NotEnoughLogs;

namespace DatabaseGenerator.FromPages;

public partial class PageImporter
{
    private void ImportFromProfilePage(Logger logger, HtmlDocument document, string filePath)
    {
        HtmlNode contents = document.DocumentNode.SelectSingleNode("//body/div[@id='page']/div[@id='contents']");
        HtmlNode profileDetail = contents.SelectSingleNode("./div[@id='profileDetail']");
        HtmlNode userNameTag = profileDetail.SelectSingleNode("./h1");

        string profileName;
        Guid? identityId = null;
        int? levelsPlayed = null;
        int? levelsPublished = null;

        if (userNameTag == null)
        {
            // Case where the page says "Game data is currently unavailable."
            // We will get the username from the endpoint then!
            profileName = PageHelper.GetEndpoint(document)![9..];
        }
        else
        {
            profileName = userNameTag.InnerText;

            HtmlNodeCollection? meta = profileDetail.SelectSingleNode("./p[@class='meta']").SelectNodes("./strong");
            levelsPlayed = meta[0].InnerText.ToInt();
            levelsPublished = meta[1].InnerText.ToInt();

            identityId = Guid.Parse(profileDetail.SelectSingleNode("./form[@class='btnFollow']/fieldset/input[@class='userId']")
                .GetAttributeValue("value", null).Split("/~identity:").Last());
        }

        PageProfile profile = new PageProfile
        {
            Name = profileName,
            IdentityId = identityId,
            LevelsPlayed = levelsPlayed,
            LevelsPublished = levelsPublished,
            Source = filePath
        };
        this.Profiles.Add(profile);

        HtmlNode mediaSetsCarousel =
            contents.SelectSingleNode("./div[@id='publishedLevelsCarousel']/div[@id='mediaSetsCarousel']");

        // If user has published any levels
        if (mediaSetsCarousel != null)
        {
            var thumbList = mediaSetsCarousel
                .SelectSingleNode("./div[@class='mediaSet mediaSet1']/div[@class='scrollable']/ul[@class='items']")
                .SelectNodes("./li");

            foreach (var li in thumbList)
            {
                var levelThumbnail = li.SelectSingleNode("./a");
                string levelId = levelThumbnail.GetAttributeValue("href", "").Split("/").Last();
                Guid resourceGuid = levelThumbnail.SelectSingleNode("./img").GetAttributeValue("src", "")
                    .GetResourceFromThumbnailSrc();

                string levelName = li.SelectSingleNode("./h3").InnerText;

                try
                {
                    var stats = li.SelectSingleNode("./p").SelectNodes("./strong");
                    int plays = stats[0].InnerText.ToInt();
                    int likes = stats[1].InnerText.ToInt();
                    var level = new PageLevel
                    {
                        ResourceGuid = resourceGuid,
                        Name = levelName,
                        UserName = profileName,
                        Plays = plays,
                        Likes = likes,
                        AverageLives = null,
                        Id = levelId,
                        VersionTimestamp = null,
                        Source = filePath
                    };


                    this.Levels.Add(level);
                }
                catch (Exception e)
                {
                    logger.LogWarning(LogContext.PageImport, $"{profileName} has a corrupt profile page and their import will therefore be incomplete.");
                    
                    // This is for one specific page that randomly abruptly ends here (DJIMAGE1 page may 14 2016 is corrupt). Trying to salvage the data that we could get before returning 
                    var level = new PageLevel
                    {
                        ResourceGuid = resourceGuid,
                        Name = levelName,
                        UserName = profileName,
                        Plays = null,
                        Likes = null,
                        AverageLives = null,
                        Id = levelId,
                        VersionTimestamp = null,
                        Source = filePath
                    };


                    this.Levels.Add(level);
                    return;
                }
            }
        }

        HtmlNode leftColumn = contents.SelectSingleNode("./div[@id='leftColumn']");
        var thumbListLikes = leftColumn.SelectSingleNode("./div[@class='panel col3']/ul[@class='thumbList']");

        // User has liked any levels
        if (thumbListLikes != null)
        {
            var levelLis = thumbListLikes.SelectNodes("./li");
            foreach (var li in levelLis)
            {
                HtmlNode? levelThumbnail = li.SelectSingleNode("./a");
                string levelId =
                    levelThumbnail.GetAttributeValue("href", "").Split("/").Last();
                Guid resourceGuid = levelThumbnail.SelectSingleNode("./img").GetAttributeValue("src", "")
                    .GetResourceFromThumbnailSrc();

                string levelName = li.SelectSingleNode("./h3").InnerText;
                HtmlNode? authorTag = li.SelectSingleNode("./p/a");
                string? authorName = authorTag?.InnerText;

                PageLevel level = new PageLevel
                {
                    ResourceGuid = resourceGuid,
                    Name = levelName,
                    UserName = authorName,
                    Plays = null,
                    Likes = null,
                    AverageLives = null,
                    Id = levelId,
                    VersionTimestamp = null,
                    Source = filePath
                };
                this.Levels.Add(level);
            }
        }

        HtmlNode activityStream =
            contents.SelectSingleNode(
                "./div[@id='mainColumn']/div[@class='panel stream']/ol[@class='activityStream']");

        // If user has any activities
        if (activityStream != null)
        {
            var activityLis = activityStream.SelectNodes("./li");
            foreach (var li in activityLis)
            {
                var h3 = li.SelectSingleNode("./h3");
                string activityUserName = h3.SelectSingleNode("./a").InnerText;
                string verb = h3.SelectSingleNode("./span").InnerText;

                DateTimeOffset date = DateTimeOffset.Parse(li.SelectSingleNode("./p[@class='timeStamp']/span[@class='cutetime']")
                    .GetAttributeValue("data-timestamp", ""));

                HtmlNode levelInfo = li.SelectSingleNode("./div[@class='levelInfo']");
                string? levelId = null;
                long? commentId = null;

                EventType eventType = PageHelper.GetEventType(verb);

                switch (eventType)
                {
                    case EventType.LevelCommentReply:
                    case EventType.LevelComment:
                    case EventType.ArticleCommentReply:
                    case EventType.ArticleComment:
                        string formattedCommentId = li.GetAttributeValue("id", "");
                        (commentId, long? replyId) = PageHelper.GetCommentIdsFromFormatted(formattedCommentId);
                        
                        string text = li.SelectSingleNode("./div[@class='comment']").InnerText.Trim();
                        Guid? articleId = null;
                        string? articleName = null;
                        string? articleLink = null;
                        string? articleSummary = null;
                        string? articleIconUrl = null;

                        if (eventType is EventType.ArticleComment or EventType.ArticleCommentReply)
                        {
                            articleId = PageHelper.GetArticleIdFromFormatted(formattedCommentId);
                            articleLink = levelInfo.SelectSingleNode("./a").GetAttributeValue("href", "");
                            articleIconUrl = levelInfo.SelectSingleNode("./a/img").GetAttributeValue("src", "");
                            
                            articleName = levelInfo.SelectSingleNode("./h4").InnerText;
                            articleSummary = levelInfo.SelectSingleNode("./p[@class='meta']").InnerText;
                        }
                        else
                        {
                            levelId = PageHelper.GetLevelIdFromFormatted(formattedCommentId);
                        }
                        
                        var comment = new PageComment
                        {
                            UserName = profileName,
                            Text = text,
                            ArticleId = articleId,
                            LevelId = levelId,
                            Timestamp = (long)commentId,
                            SecondTimestamp = replyId,
                            Source = filePath,
                            ArticleName = articleName,
                            ArticleContentUrl = articleLink,
                            ArticleSummary = articleSummary,
                            ArticleIconUrl = articleIconUrl
                        };
                        this.Comments.Add(comment);
                        break;
                    case EventType.LevelLike:
                    case EventType.LevelPublish:
                        var levelThumbnail = levelInfo.SelectSingleNode("./a");
                        levelId = levelThumbnail.GetAttributeValue("href", "").Split("/").Last();
                        Guid levelResourceGuid = levelThumbnail.SelectSingleNode("./img").GetAttributeValue("src", "")
                            .GetResourceFromThumbnailSrc();

                        string levelName = levelInfo.SelectSingleNode("./h4").InnerText;

                        HtmlNode meta = levelInfo.SelectSingleNode("./p[@class='meta']");
                        string levelAuthorName = meta.SelectSingleNode("./a").InnerText;

                        // by fireburn95 1,489 plays, 303 likes
                        string[] splitMeta = meta.InnerText.Split(' ');
                        int levelPlays = splitMeta[2].ToInt();
                        int levelLikes = splitMeta[4].ToInt();

                        var level = new PageLevel
                        {
                            ResourceGuid = levelResourceGuid,
                            Name = levelName,
                            UserName = levelAuthorName,
                            Plays = levelPlays,
                            Likes = levelLikes,
                            AverageLives = null,
                            Id = levelId,
                            VersionTimestamp = null,
                            Source = filePath
                        };
                        this.Levels.Add(level);
                        break;
                }
                
                var activity = new PageEvent
                {
                    UserName = activityUserName,
                    LevelId = levelId,
                    CommentId = commentId,
                    EventType = eventType,
                    CreationDate = date,
                    Source = filePath
                };
                this.Events.Add(activity);
            }
        }
    }
}