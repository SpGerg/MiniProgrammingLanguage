using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class RoundNumberValue : AbstractValue
{
    public RoundNumberValue(int value)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.RoundNumber;

    public override ValueType[] CanCast { get; } = { ValueType.String, ValueType.Number };
    
    public int Value { get; }
    
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
        return Value;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.RoundNumber.ToString(), ValueType.Boolean.ToString(), location);
        
        return false;
    }
}