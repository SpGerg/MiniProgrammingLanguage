using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class StringValue : AbstractValue
{
    public StringValue(string value)
    {
        Value = value;
    }
    
    public override ValueType Type => ValueType.String;
    
    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };
    
    public string Value { get; }
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }
    
    public override string AsString(ProgramContext programContext, Location location)
    {
        return Value;
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        if (!float.TryParse(Value, out var value))
        {
            InterpreterThrowHelper.ThrowCannotCastException(ValueType.String.ToString(), ValueType.Number.ToString(), location);
        }

        return value;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        if (!int.TryParse(Value, out var value))
        {
            InterpreterThrowHelper.ThrowCannotCastException(ValueType.String.ToString(), ValueType.RoundNumber.ToString(), location);
        }

        return value;
    }
}