namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class IncorrectTypeException : AbstractInterpreterException
{
    public IncorrectTypeException(string excepted, string received, Location location) : base($"Expected '{excepted}', but got '{received}'", location)
    {
    }
}