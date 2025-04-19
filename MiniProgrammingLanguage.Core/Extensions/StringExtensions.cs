namespace MiniProgrammingLanguage.Core.Extensions;

public static class StringExtensions
{
    public static Location GetLocationByPosition(this string source, int position)
    {
        var subString = source.Substring(0, position);
        var split = subString.Split('\n');
        var line = split.Length;
        var column = subString.Length - subString.LastIndexOf('\n') - 1;

        return new Location()
        {
            Line = line,
            Position = column
        };
    }
}