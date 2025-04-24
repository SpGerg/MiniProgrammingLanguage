using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Exceptions;

namespace MiniProgrammingLanguage.SharpKit.Exceptions;

public class ModuleNotFoundException : AbstractLanguageException
{
    public ModuleNotFoundException(string module, Location location) : base($"Module with '{module}' name not found", location)
    {
    }
}