namespace MiniProgrammingLanguage.Core.Parser.Ast;

public abstract class AbstractExpression
{
    protected AbstractExpression(Location location)
    {
        Location = location;
    }

    public Location Location { get; }
}