using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Parser.Ast;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class ArrayValue : AbstractValue
{
    public ArrayValue(IEnumerable<AbstractEvaluableExpression> value)
    {
        Value = value;
    }
    
    public override ValueType Type => ValueType.Array;

    public override ValueType[] CanCast { get; } = { ValueType.String };

    public IEnumerable<AbstractEvaluableExpression> Value { get; }
    
    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"({Value.Count()}) [ ");

        foreach (var value in Value)
        {
            var result = value.Evaluate(programContext);
            var message = result is NoneValue ? "none" : $"{result.Type.ToString().ToLower()}: {result.AsString(programContext, location)}";
            
            stringBuilder.Append(message);
            
            if (value == Value.Last())
            {
                continue;
            }

            stringBuilder.Append(", ");
        }
        
        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Array.ToString(), ValueType.Number.ToString(), location);

        return -1;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Array.ToString(), ValueType.RoundNumber.ToString(), location);

        return -1;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Array.ToString(), ValueType.Boolean.ToString(), location);

        return false;
    }
}