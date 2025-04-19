using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class NumberExpression : AbstractEvaluableExpression
{
    public NumberExpression(float value, Location location) : base(location)
    {
        Value = value;
    }

    public override bool IsValue => true;

    public float Value { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new NumberValue(Value);
    }
}