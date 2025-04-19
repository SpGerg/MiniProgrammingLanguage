using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public abstract class AbstractValue
{
    public abstract ValueType Type { get; }
    
    public abstract ValueType[] CanCast { get; }

    public abstract string AsString(ProgramContext programContext, Location location);
    
    public abstract float AsNumber(ProgramContext programContext, Location location);
    
    public abstract int AsRoundNumber(ProgramContext programContext, Location location);
    
    public abstract bool AsBoolean(ProgramContext programContext, Location location);

    public bool Is(ObjectTypeValue objectTypeValue)
    {
        if (objectTypeValue.ValueType is ValueType.Any)
        {
            return true;
        }

        return objectTypeValue.ValueType == Type;
    }
    
    public virtual bool Is(AbstractValue objectTypeValue)
    {
        if (Type is ValueType.Any)
        {
            return true;
        }

        return objectTypeValue.Type == Type;
    }

    public AbstractValue Cast(ProgramContext programContext, ValueType valueType, Location location)
    {
        return valueType switch
        {
            ValueType.Number => new NumberValue(AsNumber(programContext, location)),
            ValueType.Boolean => new BooleanValue(AsBoolean(programContext,location)),
            ValueType.String => new StringValue(AsString(programContext,location)),
            _ => null
        };
    }

    public override string ToString()
    {
        return Type.ToString();
    }
}