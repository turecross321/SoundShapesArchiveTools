namespace DatabaseGenerator.FromPages;

public static class StringExtensions
{
    public static int ToInt(this string value)
    {
        return int.Parse(value.Replace(",", ""));
    }
}