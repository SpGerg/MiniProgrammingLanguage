using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class TypeCompatibilityVisitor : IValueVisitor
{
    public TypeCompatibilityVisitor(ObjectTypeValue objectTypeValue)
    {
        Type = objectTypeValue;
    }
    
    public ObjectTypeValue Type { get; }

    public bool Visit(TypeValue typeValue)
    {
        if (Type.ValueType is ValueType.Object)
        {
            return true;
        }
        
        return Type.ValueType is ValueType.Type && typeValue.Name == Type.Name;
    }

    public bool Visit(ArrayValue arrayValue)
    {
        return Type.ValueType is ValueType.Array;
    }

    public bool Visit(BooleanValue booleanValue)
    {
        return Type.ValueType is ValueType.Boolean;
    }

    public bool Visit(FunctionValue functionValue)
    {
        return Type.ValueType is ValueType.Function;
    }

    public bool Visit(NoneValue noneValue)
    {
        return Type.ValueType is ValueType.None;
    }

    public bool Visit(CSharpObjectValue cSharpObjectValue)
    {
        return Type.ValueType is ValueType.CSharpObject;
    }

    public bool Visit(NumberValue numberValue)
    {
        return Type.ValueType is ValueType.Number;
    }

    public bool Visit(ObjectTypeValue objectTypeValue)
    {
        if (objectTypeValue.ValueType is ValueType.Type && Type.ValueType is ValueType.Object)
        {
            return true;
        }
        
        return Type.ValueType == objectTypeValue.ValueType && Type.Name == objectTypeValue.Name;
    }

    public bool Visit(RoundNumberValue roundNumberValue)
    {
        return Type.ValueType is ValueType.RoundNumber or ValueType.Number;
    }

    public bool Visit(EnumValue enumValue)
    {
        if (string.IsNullOrEmpty(Type.Name))
        {
            return Type.ValueType is ValueType.Enum;
        }
        
        return Type.ValueType is ValueType.Enum && Type.Name == enumValue.Value.Name;
    }

    public bool Visit(StringValue stringValue)
    {
        return Type.ValueType is ValueType.String or ValueType.None;
    }

    public bool Visit(EnumMemberValue enumMemberValue)
    {
        if (string.IsNullOrEmpty(Type.Name))
        {
            return Type.ValueType is ValueType.EnumMember;
        }
        
        return Type.ValueType is ValueType.EnumMember && Type.Name == enumMemberValue.Name;
    }

    public bool Visit(VoidValue voidValue)
    {
        return Type.ValueType is ValueType.Void;
    }
}