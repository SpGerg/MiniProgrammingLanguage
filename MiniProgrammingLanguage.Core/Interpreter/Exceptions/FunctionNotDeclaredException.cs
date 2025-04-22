namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class FunctionNotDeclaredException : AbstractInterpreterException
{
    public FunctionNotDeclaredException(string name, Location location) : base($"Function with {name} name not declared", location)
    {
    }
}