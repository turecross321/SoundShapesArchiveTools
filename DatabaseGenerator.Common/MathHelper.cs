namespace DatabaseGenerator.Common;

public static class MathHelper
{
    public static int? Max(int? first, int? second)
    {
        if (first == null)
            return second;

        if (second == null)
            return first;

        return Math.Max((int)first, (int)second);
    }
}