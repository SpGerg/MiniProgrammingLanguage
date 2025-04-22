using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.Core.Interpreter.Exceptions;

public class DivideByZeroException : AbstractLanguageException
{
    public DivideByZeroException(Location location) : base("Divide by zero", location)
    {
    }
}