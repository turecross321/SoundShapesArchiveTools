using DatabaseGenerator.Common;
using HtmlAgilityPack;

namespace DatabaseGenerator.FromPages;

public static class PageHelper
{
    public static string? GetEndpoint(HtmlDocument document)
    {
        HtmlNodeCollection? scriptNodes = document.DocumentNode.SelectNodes("//script");
        if (scriptNodes == null)
            return null;
        
        foreach (HtmlNode? node in scriptNodes)
        {
            if (!node.InnerText.Contains("soundshapes_url = \""))
                continue;

            return node.InnerText.Split("soundshapes_url = \"").Last().Split("\"").First();
        }

        return null;
    }

    public static PageType? GetPageType(HtmlDocument document)
    {
        string? endpoint = GetEndpoint(document);

        if (endpoint == null)
            return null;

        if (endpoint == "/home.html")
            return PageType.HomePage;
        if (endpoint.StartsWith("/community.html"))
            return PageType.CommunityPage;
        if (endpoint.StartsWith("/level/"))
            return PageType.LevelPage;
        if (endpoint.StartsWith("/profile/"))
            return PageType.ProfilePage;

        return null;
    }
    
    public static Guid GetResourceFromThumbnailSrc(this string src) // todo shouldnt be extension
    {
        // Extract the resource GUID from the thumbnail src
        return Guid.Parse(src[(src.LastIndexOf('/') + 1)..].AsSpan(0, 36));
    }


    // /~level:3yox73y8/~version:1363270203080
    public static string GetLevelIdFromFormatted(string value)
    {
        return value.Split("/~level:").Last().Split('/').First();
    }
    
    // /~level:3yox73y8/~version:1363270203080
    public static long GetVersionTimestampFromFormatted(string value)
    {
        // Deformat the level ID
        return long.Parse(value.Split("/~version:").Last());
    }

    public static Guid GetArticleIdFromFormatted(string value)
    {
        return Guid.Parse(value.Split("/~article:").Last().Split("/").First());
    }
    
    public static (long, long?) GetCommentIdsFromFormatted(string value)
    {
        string[] timestamps = value.Split("~comment:").Last().Split("-");

        long id = long.Parse(timestamps[0]);
        long? replyId = timestamps.Length >= 2 ? long.Parse(timestamps[1]) : null;
            
        return (id, replyId);
    }

    public static EventType GetEventType(string verb)
    {
        return verb switch
        {
            "commented on a level:" => EventType.LevelComment,
            "commented on an article:" => EventType.ArticleComment,
            "liked a level:" => EventType.LevelLike,
            "published a level:" => EventType.LevelPublish,
            "replied to a comment on a level:" => EventType.LevelCommentReply,
            "replied to a comment on an article:" => EventType.ArticleCommentReply,
            _ => throw new ArgumentOutOfRangeException(nameof(verb), verb, "Unknown activity type:" + verb)
        };
    }
}