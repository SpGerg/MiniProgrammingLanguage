namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public class TypeExpectedException : AbstractParserException
{
    public TypeExpectedException(Location location) : base("Type expected", location)
    {
    }
}