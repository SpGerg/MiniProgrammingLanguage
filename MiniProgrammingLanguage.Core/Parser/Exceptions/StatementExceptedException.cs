namespace MiniProgrammingLanguage.Core.Parser.Exceptions;

public class StatementExceptedException : AbstractParserException
{
    public StatementExceptedException(Location location) : base("Statement expected", location)
    {
    }
}