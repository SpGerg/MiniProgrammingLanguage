using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class NoneValue : AbstractValue
{
    public override ValueType Type => ValueType.None;

    public override ValueType[] CanCast { get; } = {};
    
    public override string AsString(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.None.ToString(), ValueType.String.ToString(), location);
        
        return null;
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.None.ToString(), ValueType.Number.ToString(), location);
        
        return -1;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.None.ToString(), ValueType.RoundNumber.ToString(), location);
        
        return -1;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.None.ToString(), ValueType.Boolean.ToString(), location);
        
        return false;
    }
}