using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class ArrayValue : AbstractValue
{
    public ArrayValue(AbstractValue[] value) : base(string.Empty)
    {
        Value = value;

        _count = Value.Count();
        _last = Value.Last();
    }
    
    public ArrayValue(IEnumerable<AbstractValue> value) : base(string.Empty)
    {
        Value = value.ToArray();

        _count = Value.Count();
        _last = Value.Last();
    }

    public override ValueType Type => ValueType.Array;

    public override ValueType[] CanCast { get; } = { ValueType.String };

    public override bool IsValueType => false;

    public AbstractValue[] Value { get; }

    private readonly int _count;

    private readonly AbstractValue _last;

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new ArrayValue(Value);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"({_count}) [ ");

        foreach (var value in Value)
        {
            var message = value is NoneValue
                ? "none"
                : $"{value.Type.ToString().ToLower()}: {value.AsString(programContext, location)}";

            stringBuilder.Append(message);

            if (value == _last)
            {
                continue;
            }

            stringBuilder.Append(", ");
        }

        stringBuilder.Append(" ]");

        return stringBuilder.ToString();
    }
}