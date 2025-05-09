using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class VoidExpression : AbstractEvaluableExpression
{
    public VoidExpression(Location location) : base(location)
    {
    }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return VoidValue.Instance;
    }
}