using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class DuplicateNameException : AbstractLanguageException
{
    public DuplicateNameException(string name, Location location) : base($"Object with {name} name already created", location)
    {
    }
}