namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class WrongImportModuleException : AbstractInterpreterException
{
    public WrongImportModuleException(string module, Location location) : base($"Wrong module with '{module}' name",
        location)
    {
    }
}