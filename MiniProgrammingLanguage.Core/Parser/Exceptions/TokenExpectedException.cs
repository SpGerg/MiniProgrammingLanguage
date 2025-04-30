namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public class TokenExpectedException : AbstractParserException
{
    public TokenExpectedException(string message, Location location) : base($"Except {message}", location)
    {
    }
}