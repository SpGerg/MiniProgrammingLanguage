namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class CannotCastException : AbstractInterpreterException
{
    public CannotCastException(string from, string to, Location location) : base($"Cannot cast from '{from}' to '{to}'",
        location)
    {
    }
}