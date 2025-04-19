namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class CannotAccessException : AbstractInterpreterException
{
    public CannotAccessException(string name, Location location) : base($"Cannot access object with '{name}' name", location)
    {
    }
}