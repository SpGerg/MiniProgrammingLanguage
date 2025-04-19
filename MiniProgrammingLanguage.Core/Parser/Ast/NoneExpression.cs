using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class NoneExpression : AbstractEvaluableExpression
{
    public NoneExpression(Location location) : base(location)
    {
    }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new NoneValue();
    }
}