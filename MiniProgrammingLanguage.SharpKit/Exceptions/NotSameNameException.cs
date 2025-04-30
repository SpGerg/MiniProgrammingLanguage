using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.SharpKit.Exceptions;

public class NotSameNameException : AbstractLanguageException
{
    public NotSameNameException(string type, string className, Location location) : base($"Except '{className}' name, but got '{type}'", location)
    {
    }
}