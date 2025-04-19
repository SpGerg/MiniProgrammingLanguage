namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class MemberNotFoundException : AbstractInterpreterException
{
    public MemberNotFoundException(string type, string member, Location location) : base($"Member with '{member}' name in type '{type}' not found", location)
    {
    }
}