namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public class InvalidNumberFormatException : AbstractParserException
{
    public InvalidNumberFormatException(string number, Location location) : base($"Invalid number format: {number}",
        location)
    {
    }
}