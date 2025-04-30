namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class CyclicImportException : AbstractInterpreterException
{
    public CyclicImportException(string module, Location location) : base($"Cyclic import module with {module} name",
        location)
    {
    }
}