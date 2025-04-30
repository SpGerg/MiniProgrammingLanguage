namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class ArgumentExceptedException : AbstractInterpreterException
{
    public ArgumentExceptedException(string name, Location location) : base($"Except argument with '{name}' name",
        location)
    {
    }
}