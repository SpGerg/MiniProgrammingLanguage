using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;

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

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Number.ToString(), ValueType.Boolean.ToString(), location);
        
        return false;
    }
}