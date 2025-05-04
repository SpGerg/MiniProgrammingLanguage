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

    /// <summary>
    /// Value type
    /// </summary>
    public abstract ValueType Type { get; }

    /// <summary>
    /// Castable types
    /// </summary>
    public abstract ValueType[] CanCast { get; }

    /// <summary>
    /// Is value type.
    /// Boolean, number, round number, string, none, void, function
    /// </summary>
    public virtual bool IsValueType => true;

    /// <summary>
    /// Name.
    /// it empty in: string, boolean, round number, number, none, void, function, array.
    /// In <see cref="CSharpObjectValue"/> it contains type name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Visit
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract bool Visit(IValueVisitor visitor);

    /// <summary>
    /// Gives copy of value
    /// </summary>
    /// <returns></returns>
    public abstract AbstractValue Copy();

    /// <summary>
    /// Return string of value otherwise exception
    /// </summary>
    /// <param name="programContext"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public virtual string AsString(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.String.ToString(), location);

        return null;
    }

    /// <summary>
    /// Return number of value otherwise exception
    /// </summary>
    /// <param name="programContext"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public virtual float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.Number.ToString(), location);

        return -1;
    }

    /// <summary>
    /// Return round number of value otherwise exception
    /// </summary>
    /// <param name="programContext"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public virtual int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.RoundNumber.ToString(), location);

        return -1;
    }

    /// <summary>
    /// Return boolean of value otherwise exception
    /// </summary>
    /// <param name="programContext"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public virtual bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(Type.ToString(), ValueType.Boolean.ToString(), location);

        return false;
    }

    /// <summary>
    /// Is value match by given type
    /// </summary>
    /// <param name="objectTypeValue">Type</param>
    /// <returns></returns>
    public bool Is(ObjectTypeValue objectTypeValue)
    {
        if (objectTypeValue.ValueType is ValueType.Any)
        {
            return true;
        }

        var visitor = new TypeCompatibilityVisitor(objectTypeValue);

        return Visit(visitor);
    }

    /// <summary>
    /// Is value match by value
    /// </summary>
    /// <param name="abstractValue">Value</param>
    /// <returns></returns>
    public bool Is(AbstractValue abstractValue)
    {
        if (this is ObjectTypeValue objectTypeValue)
        {
            return abstractValue.Is(objectTypeValue);
        }

        var visitor = new ValueCompatibilityVisitor(abstractValue);

        return Visit(visitor);
    }

    /// <summary>
    /// Cast value by given type
    /// </summary>
    /// <param name="programContext"></param>
    /// <param name="valueType"></param>
    /// <param name="location"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Return value type, can be also with type name.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.IsNullOrEmpty(Name) ? Type.ToString() : $"({Name}), {Type}";
    }
}