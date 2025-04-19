using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class AbstractInterpreterException : AbstractLanguageException
{
    public AbstractInterpreterException(string message, Location location) : base(message, location)
    {
    }
}