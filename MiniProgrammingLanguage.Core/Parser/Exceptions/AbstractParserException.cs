using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public abstract class AbstractParserException : AbstractLanguageException
{
    protected AbstractParserException(string message, Location location) : base(message, location)
    {
    }
}