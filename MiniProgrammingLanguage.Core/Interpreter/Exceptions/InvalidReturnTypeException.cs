using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class InvalidReturnTypeException : AbstractLanguageException
{
    public InvalidReturnTypeException(string function, string excepted, string returned, Location location) : base(
        $"Function with '{function}' name, return '{returned}' but expected '{excepted}'", location)
    {
    }
}