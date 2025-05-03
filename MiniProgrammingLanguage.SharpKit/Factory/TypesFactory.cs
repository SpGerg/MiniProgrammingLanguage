using System;
using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class TypesFactory
{
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
            var type = target.GetType();

            if (type.IsEnum)
            {
                var index = type.GetField("value__");

                if (index is not null)
                {
                    var enumInstance = programContext.Enums.Get(null, type.Name, programContext.Module, Location.Default);
                    
                    implementModule = null;
                    return new EnumMemberValue(enumInstance, type.GetEnumName(index.GetValue(target)));
                }
                
                var enumValue = EnumCreator.Create(type);

                implementModule = new ImplementModule
                {
                    Name = "global",
                    Enums = new [] { enumValue.Value }
                };
                
                return enumValue;
            }

            return TypeCreator.Create(type, programContext, target, out implementModule);
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

        if (result is null)
        {
            if (target is TypeValue typeValue)
            {
                return typeValue.ObjectTarget;
            }
            
            if (target is EnumValue enumValue)
            {
                return enumValue.Type;
            }
            
            if (target is EnumMemberValue enumMemberValue)
            {
                return Enum.Parse(enumMemberValue.Parent.Type, enumMemberValue.Member);
            }
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

        if (target.IsEnum)
        {
            var enumValue = EnumCreator.Create(target);
            var objectType = new ObjectTypeValue(enumValue.Name, ValueType.EnumMember);

            if (programContext.Enums.Get(null, enumValue.Name, programContext.Module, Location.Default) is null)
            {
                implementModule = new ImplementModule
                {
                    Name = "global",
                    Enums = new [] { enumValue.Value },
                    Variables = new[]
                    {
                        new UserVariableInstance
                        {
                            Name = enumValue.Name,
                            Module = programContext.Module,
                            Type = new ObjectTypeValue(enumValue.Name, ValueType.Enum),
                            Access = AccessType.Static | AccessType.ReadOnly,
                            Value = enumValue,
                            Root = null
                        }
                    }
                };
            }
            else
            {
                implementModule = null;
            }

            return objectType;
        }
        
        var type = TypeCreator.Create(target, programContext, objectTarget, out implementModule);

        return new ObjectTypeValue(type.Name, ValueType.Type);
    }
}