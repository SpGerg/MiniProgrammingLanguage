using System;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public abstract class AbstractValue
{
    protected AbstractValue(string name)
    {
        Name = name;
    }

    public abstract ValueType Type { get; }

    public abstract ValueType[] CanCast { get; }

    public virtual bool IsValueType => true;

    public string Name { get; }

    public abstract bool Visit(IValueVisitor visitor);

    public abstract AbstractValue Copy();

    public virtual string AsString(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.String.ToString(), location);

        return null;
    }

    public virtual float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.Number.ToString(), location);

        return -1;
    }

    public virtual int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.RoundNumber.ToString(), location);

        return -1;
    }

    public virtual bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.Boolean.ToString(), location);

        return false;
    }

    public bool Is(ObjectTypeValue objectTypeValue)
    {
        if (objectTypeValue.ValueType is ValueType.Any)
        {
            return true;
        }

        var visitor = new TypeCompatibilityVisitor(objectTypeValue);

        return Visit(visitor);
    }

    public bool Is(AbstractValue abstractValue)
    {
        if (this is ObjectTypeValue objectTypeValue)
        {
            return abstractValue.Is(objectTypeValue);
        }

        var visitor = new ValueCompatibilityVisitor(abstractValue);

        return Visit(visitor);
    }

    public AbstractValue Cast(ProgramContext programContext, ValueType valueType, Location location)
    {
        return valueType switch
        {
            ValueType.Number => new NumberValue(AsNumber(programContext, location)),
            ValueType.Boolean => new BooleanValue(AsBoolean(programContext, location)),
            ValueType.String => new StringValue(AsString(programContext, location)),
            _ => null
        };
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(Name) ? Type.ToString() : $"({Name}), {Type}";
    }
}