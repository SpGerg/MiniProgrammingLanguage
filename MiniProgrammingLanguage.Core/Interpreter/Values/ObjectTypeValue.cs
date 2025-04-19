using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class ObjectTypeValue : AbstractValue
{
    public static ObjectTypeValue String => new(string.Empty, ValueType.String);
    
    public static ObjectTypeValue Number => new(string.Empty, ValueType.Number);
    
    public static ObjectTypeValue Boolean => new(string.Empty, ValueType.Boolean);
    
    public static ObjectTypeValue RoundNumber => new(string.Empty, ValueType.RoundNumber);
    
    public static ObjectTypeValue Any => new(string.Empty, ValueType.Any);
    
    public static ObjectTypeValue Void => new(string.Empty, ValueType.Void);
    
    public ObjectTypeValue(string name, ValueType valueType)
    {
        Name = name;
        ValueType = valueType;
    }

    public override ValueType Type => ValueType.ObjectType;
    
    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };
    
    public string Name { get; }
    
    public ValueType ValueType { get; }

    public override bool Is(AbstractValue abstractValue)
    {
        if (ValueType is ValueType.Any)
        {
            return true;
        }

        return abstractValue switch
        {
            ObjectTypeValue objectTypeValue => ValueType == objectTypeValue.ValueType && Name == objectTypeValue.Name,
            TypeValue typeValue => ValueType is ValueType.Type && Name == typeValue.Name,
            NoneValue when ValueType is ValueType.Type => true,
            _ => ValueType == abstractValue.Type
        };
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return string.IsNullOrEmpty(Name) ? ValueType.ToString() : $"{Name}, {ValueType}";
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.ObjectType.ToString(), ValueType.Number.ToString(), location);

        return -1;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.ObjectType.ToString(), ValueType.RoundNumber.ToString(), location);
        
        return -1;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.ObjectType.ToString(), ValueType.Boolean.ToString(), location);
        
        return false;
    }

    public override string ToString()
    {
        return AsString(null, Location.Default);
    }
}