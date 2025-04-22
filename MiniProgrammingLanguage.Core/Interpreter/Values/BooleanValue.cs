using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class BooleanValue : AbstractValue
{
    public BooleanValue(bool value)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.Boolean;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String,
        ValueType.Number,
        ValueType.RoundNumber,
        ValueType.Boolean
    };

    public bool Value { get; }
    
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
        return Value ? 1 : 0;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        return Value ? 1 : 0;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        return Value;
    }
}