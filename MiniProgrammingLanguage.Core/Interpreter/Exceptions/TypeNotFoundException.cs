namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class TypeNotFoundException : AbstractInterpreterException
{
    public TypeNotFoundException(string name, Location location) : base($"Type with {name} not found", location)
    {
    }
}