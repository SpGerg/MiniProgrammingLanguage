using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class ValueCompatibilityVisitor : IValueVisitor
{
    public ValueCompatibilityVisitor(AbstractValue value)
    {
        Value = value;
    }

    public AbstractValue Value { get; }
    
    public bool Visit(TypeValue typeValue)
    {
        if (Value is not TypeValue type)
        {
            return false;
        }
        
        return type.Name == typeValue.Name;
    }

    public bool Visit(ArrayValue arrayValue)
    {
        return arrayValue == Value;
    }

    public bool Visit(BooleanValue booleanValue)
    {
        if (Value is not BooleanValue boolean)
        {
            return false;
        }
        
        return boolean.Value == booleanValue.Value;
    }

    public bool Visit(FunctionValue functionValue)
    {
        if (Value is not FunctionValue function)
        {
            return false;
        }
        
        return functionValue.Value == function.Value;
    }

    public bool Visit(NoneValue noneValue)
    {
        return Value.Type is ValueType.None;
    }

    public bool Visit(NumberValue numberValue)
    {
        if (Value is RoundNumberValue roundNumberValue)
        {
            return roundNumberValue.Value == numberValue.Value;
        }
        
        if (Value is not NumberValue number)
        {
            return false;
        }
        
        return numberValue.Value == number.Value;
    }

    public bool Visit(ObjectTypeValue objectTypeValue)
    {
        if (Value is not ObjectTypeValue objectType)
        {
            return false;
        }
        
        return objectTypeValue.ValueType == objectType.ValueType && objectTypeValue.Name == objectType.Name;
    }

    public bool Visit(RoundNumberValue roundNumberValue)
    {
        if (Value is not RoundNumberValue roundNumber)
        {
            return false;
        }
        
        return roundNumberValue.Value == roundNumber.Value;
    }

    public bool Visit(EnumValue enumValue)
    {
        if (Value is not EnumValue value)
        {
            return false;
        }

        return enumValue.Value.Name == value.Value.Name;
    }

    public bool Visit(StringValue stringValue)
    {
        if (Value is not StringValue @string)
        {
            return false;
        }
        
        return @string.Value == stringValue.Value;
    }

    public bool Visit(EnumMemberValue enumMemberValue)
    {
        if (Value is not EnumMemberValue enumMember)
        {
            return false;
        }

        return enumMemberValue.Name == enumMember.Name && enumMemberValue.Member == enumMember.Member;
    }

    public bool Visit(VoidValue voidValue)
    {
        return Value.Type is ValueType.Void;
    }
}