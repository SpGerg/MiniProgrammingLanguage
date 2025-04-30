namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class FunctionNotFoundException : AbstractInterpreterException
{
    public FunctionNotFoundException(string name, Location location) : base($"Function with '{name}' name not found",
        location)
    {
    }
}