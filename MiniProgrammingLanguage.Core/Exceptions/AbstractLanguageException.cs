using System;

namespace MiniProgrammingLanguage.Core.Exceptions;

public abstract class AbstractLanguageException : Exception
{
    protected AbstractLanguageException(string message, Location location) : base($"{message}, {location}") {}
}