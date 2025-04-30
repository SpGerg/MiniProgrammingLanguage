namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class VariableNotFoundException : AbstractInterpreterException
{
    public VariableNotFoundException(string variable, Location location) : base(
        $"Variable with '{variable}' name not found", location)
    {
    }
}