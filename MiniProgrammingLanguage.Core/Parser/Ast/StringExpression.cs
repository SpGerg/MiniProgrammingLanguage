using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class StringExpression : AbstractEvaluableExpression
{
    public StringExpression(string value, Location location) : base(location)
    {
        Value = value;
    }

    public override bool IsValue => true;
    
    public string Value { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new StringValue(Value);
    }
}