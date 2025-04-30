using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.SharpKit.Functions;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class TypesFactory
{
    public const string GeneratedMemberAttribute = "sharp_kit_generated_type";

    public static AbstractValue Create(object target, ProgramContext programContext, out ImplementModule implementModule)
    {
        AbstractValue result = target switch
        {
            string => new StringValue(Convert.ToString(target)),
            int => new RoundNumberValue(Convert.ToInt32(target)),
            float => new NumberValue(Convert.ToSingle(target)),
            bool => new BooleanValue(Convert.ToBoolean(target)),
            null => new NoneValue(),
            _ => null
        };

        if (result is null)
        {
            var type = ClassCreator.Create(target.GetType(), programContext, target, out implementModule);

            return type;
        }

        implementModule = null;
        return result;
    }

    public static object Create(AbstractValue target)
    {
        object result = target.Type switch
        {
            ValueType.String => target.AsString(null, Location.Default),
            ValueType.RoundNumber => target.AsRoundNumber(null, Location.Default),
            ValueType.Number => target.AsNumber(null, Location.Default),
            ValueType.Boolean => target.AsBoolean(null, Location.Default),
            ValueType.Void => typeof(void),
            _ => null,
        };

        if (result is null && target is TypeValue typeValue)
        {
            return typeValue.ObjectTarget;
        }
        
        return result;
    }
    
    public static ObjectTypeValue GetObjectTypeByType(Type target, ProgramContext programContext, object objectTarget, out ImplementModule implementModule)
    {
        if (target == typeof(string))
        {
            implementModule = null;
            return ObjectTypeValue.String;
        }
        
        if (target == typeof(int))
        {
            implementModule = null;
            return ObjectTypeValue.RoundNumber;
        }
        
        if (target == typeof(bool))
        {
            implementModule = null;
            return ObjectTypeValue.Boolean;
        }
        
        if (target == typeof(object))
        {
            implementModule = null;
            return ObjectTypeValue.Object;
        }
        
        if (target == typeof(void))
        {
            implementModule = null;
            return ObjectTypeValue.Void;
        }
        
        var type = ClassCreator.Create(target, programContext, objectTarget, out implementModule);

        return new ObjectTypeValue(type.Name, ValueType.Type);
    }
}