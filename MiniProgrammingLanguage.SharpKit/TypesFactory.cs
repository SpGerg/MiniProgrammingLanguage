using System;
using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit;

public static class TypesFactory
{
    public static AbstractValue Create(object target)
    {
        return target switch
        {
            string => new StringValue(Convert.ToString(target)),
            int => new RoundNumberValue(Convert.ToInt32(target)),
            float => new NumberValue(Convert.ToSingle(target)),
            bool => new BooleanValue(Convert.ToBoolean(target)),
            _ => null
        };
    }
    
    public static object Create(AbstractValue target)
    {
        return target.Type switch
        {
            ValueType.String => target.AsString(null, Location.Default),
            ValueType.RoundNumber => target.AsRoundNumber(null, Location.Default),
            ValueType.Number => target.AsNumber(null, Location.Default),
            ValueType.Boolean => target.AsBoolean(null, Location.Default),
            _ => null
        };
    }
}