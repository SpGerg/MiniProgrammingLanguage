namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public class ValueExpectedException : AbstractParserException
{
    public ValueExpectedException(Location location) : base("Expect values, variables or function", location)
    {
    }
}