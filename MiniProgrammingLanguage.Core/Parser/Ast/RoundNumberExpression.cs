using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class RoundNumberExpression : AbstractEvaluableExpression
{
    public RoundNumberExpression(int value, Location location) : base(location)
    {
        Value = value;
    }

    public override bool IsValue => true;
    
    public int Value { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new RoundNumberValue(Value);
    }
}