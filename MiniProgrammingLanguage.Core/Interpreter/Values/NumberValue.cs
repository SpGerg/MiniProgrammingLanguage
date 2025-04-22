using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class NumberValue : AbstractValue
{
    public NumberValue(float value)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.Number;
    
    public override ValueType[] CanCast { get; } =
    {
        ValueType.String,
        ValueType.Number,
        ValueType.RoundNumber
    };
    
    public float Value { get; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return Value.ToString();
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        return Value;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        return (int)Value;
    }
}