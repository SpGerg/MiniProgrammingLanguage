using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class BooleanExpression : AbstractEvaluableExpression
{
    public BooleanExpression(bool value, Location location) : base(location)
    {
        Value = value;
    }

    public bool Value { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new BooleanValue(Value);
    }
}