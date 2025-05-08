using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ValueExpression : AbstractEvaluableExpression
{
    public ValueExpression(AbstractValue value, Location location) : base(location)
    {
        Value = value;
    }

    public AbstractValue Value { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return Value;
    }
}