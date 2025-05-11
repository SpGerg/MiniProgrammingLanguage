using System;
using System.Linq;
using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.SharpKit.Ast;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class TypesFactory
{
    /// <summary>
    /// Create type by CSharp type
    /// </summary>
    /// <param name="target"></param>
    /// <param name="programContext"></param>
    /// <param name="implementModule">Types created in process of creating thi</param>
    /// <returns></returns>
    public static AbstractValue Create(object target, ProgramContext programContext, out ImplementModule implementModule)
    {
        AbstractValue result = target switch
        {
            string => new StringValue(Convert.ToString(target)),
            int => new RoundNumberValue(Convert.ToInt32(target)),
            float => new NumberValue(Convert.ToSingle(target)),
            bool => new BooleanValue(Convert.ToBoolean(target)),
            null => NoneValue.Instance,
            _ => null
        };

        if (result is null)
        {
            if (target is Array array)
            {
                var expressions = new AbstractValue[array.Length];

                for (var i = 0; i < array.Length; i++)
                {
                    expressions[i] = Create(array.GetValue(i), programContext, out implementModule);
                    programContext.Import(implementModule);
                }

                implementModule = null;
                return new ArrayValue(expressions);
            }

            var type = target.GetType();

            if (type.IsEnum)
            {
                var index = type.GetField("value__");

                if (index is not null)
                {
                    var enumInstance = programContext.Enums.Get(null, type.Name, programContext.Module, Location.Default);
                    
                    implementModule = null;
                    return new EnumMemberValue(enumInstance, type.GetEnumName(index.GetValue(target)), (int) index.GetValue(target));
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

    /// <summary>
    /// Create CSharp type by MPL type
    /// </summary>
    /// <param name="target"></param>
    /// <param name="programContext"></param>
    /// <returns></returns>
    public static object Create(AbstractValue target)
    {
        if (target.Type is ValueType.None)
        {
            return null;
        }
        
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
            if (target is ArrayValue arrayValue)
            {
                var firstElement = Create(arrayValue.Value.ElementAt(0));
                var array = Array.CreateInstance(firstElement.GetType(), arrayValue.Value.Count());
                array.SetValue(firstElement, 0);

                for (var i = 1; i < array.Length; i++)
                {
                    var value = Create(arrayValue.Value.ElementAt(i));
                    
                    array.SetValue(value, i);
                }

                return array;
            }
            
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

    /// <summary>
    /// Get MPL type by CSharp type
    /// </summary>
    /// <param name="target"></param>
    /// <param name="programContext"></param>
    /// <param name="objectTarget"></param>
    /// <param name="implementModule">Types created in process of creating thi</param>
    /// <returns></returns>
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