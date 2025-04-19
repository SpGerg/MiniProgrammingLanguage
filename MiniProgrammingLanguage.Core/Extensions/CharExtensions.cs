namespace MiniProgrammingLanguage.Core.Extensions;

public static class CharExtensions
{
    public static bool IsSpace(this char value)
    {
        return value is ' ';
    }
}