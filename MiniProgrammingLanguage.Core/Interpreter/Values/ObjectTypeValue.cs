using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
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
    
    public static ObjectTypeValue Function => new(string.Empty, ValueType.Function);
    
    public static ObjectTypeValue Object => new(string.Empty, ValueType.Object);
    
    public static ObjectTypeValue CSharpObject => new(string.Empty, ValueType.CSharpObject);
    
    public static ObjectTypeValue TypeInstance => new(string.Empty, ValueType.TypeInstance);
    
    public ObjectTypeValue(string name, ValueType valueType) : base(name)
    {
        Name = name;
        ValueType = valueType;
    }

    public override ValueType Type => ValueType.ObjectType;
    
    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };
    
    public string Name { get; } = string.Empty;
    
    public ValueType ValueType { get; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }
    
    public override AbstractValue Copy()
    {
        return new ObjectTypeValue(Name, ValueType);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return string.IsNullOrEmpty(Name) ? ValueType.ToString() : $"{Name}, {ValueType}";
    }

    public override string ToString()
    {
        return AsString(null, Location.Default);
    }
}