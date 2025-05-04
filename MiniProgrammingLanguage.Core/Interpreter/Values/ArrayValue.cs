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
    public ArrayValue(AbstractEvaluableExpression[] value) : base(string.Empty)
    {
        Value = value;

        _count = Value.Count();
        _last = Value.Last();
    }

    public override ValueType Type => ValueType.Array;

    public override ValueType[] CanCast { get; } = { ValueType.String };

    public override bool IsValueType => false;

    public AbstractEvaluableExpression[] Value { get; }

    private readonly int _count;

    private readonly AbstractEvaluableExpression _last;

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
            var result = value.Evaluate(programContext);
            var message = result is NoneValue
                ? "none"
                : $"{result.Type.ToString().ToLower()}: {result.AsString(programContext, location)}";

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