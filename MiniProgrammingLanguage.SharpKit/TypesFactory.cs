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

namespace MiniProgrammingLanguage.SharpKit;

public static class TypesFactory
{
    public const string GeneratedMemberAttribute = "sharp_kit_generated_type";
    
    private static readonly Type asyncAttribute = typeof(AsyncStateMachineAttribute);
    
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
            var types = new List<ITypeInstance>();
            var type = CreateClassOrStructureByObject(target, programContext, ref types);

            implementModule = new ImplementModule
            {
                Name = "global",
                Types = types
            };

            return type;
        }

        implementModule = null;
        return result;
    }

    public static object Create(AbstractValue target, ProgramContext programContext)
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
            return typeValue.ObjectTarget ?? (typeValue.ObjectTarget = Activator.CreateInstance(typeValue.Value.Type));
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

        var types = new List<ITypeInstance>();
        var type = CreateClassOrStructureByType(target, programContext, objectTarget, ref types);

        implementModule = new ImplementModule
        {
            Name = "global",
            Types = types
        };

        return new ObjectTypeValue(type.Name, ValueType.Type);
    }

    public static ObjectTypeValue GetObjectTypeByObject(object target, ProgramContext programContext, out ImplementModule implementModule)
    {
        return GetObjectTypeByType(target.GetType(), programContext, target, out implementModule);
    }

    public static AbstractValue CreateClassOrStructureByObject(object target, ProgramContext programContext, ref List<ITypeInstance> types)
    {
        return CreateClassOrStructureByType(target.GetType(), programContext, target, ref types);
    }
    
    public static AbstractValue CreateClassOrStructureByType(Type type, ProgramContext programContext, object objectTarget, ref List<ITypeInstance> types)
    {
        var exists = types.FirstOrDefault(entity => entity.Name == type.Name);

        if (exists is null)
        {
            exists = programContext.Types.Entities.FirstOrDefault(entity => entity.Name == type.Name);
        }

        if (exists is not null)
        {
            var existsValue = exists.Create();
            existsValue.ObjectTarget = objectTarget;

            return existsValue;
        }
        
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var members = new List<ITypeMember>();

        foreach (var method in methods)
        {
            if (method.Name == nameof(GetType) || method.IsSpecialName)
            {
                continue;
            }

            var member = CreateFunctionMemberByMethod(method, programContext, objectTarget, ref types);
            
            members.Add(member);
            members.Add(CreateVariableOfFunctionMemberByProperty(member));
        }
        
        foreach (var property in properties)
        {
            members.Add(CreateVariableMemberByProperty(property, programContext, objectTarget, ref types));
        }

        var typeInstance = new UserTypeInstance
        {
            Name = type.Name,
            Module = type.Assembly.GetName().Name,
            Members = members,
            Type = type,
            Access = AccessType.Static,
            Root = null,
        };

        types.Add(typeInstance);
        
        var result = new TypeValue(typeInstance)
        {
            ObjectTarget = objectTarget
        };

        return result;
    }

    public static TypeLanguageVariableMemberInstance CreateVariableOfFunctionMemberByProperty(ITypeLanguageFunctionMember functionMember)
    {
        return new TypeLanguageVariableMemberInstance
        {
            Parent = functionMember.Parent,
            Module = functionMember.Module,
            Identification = new KeyTypeMemberIdentification
            {
                Identifier = functionMember.Identification.Identifier
            },
            GetBind = _ => functionMember.Create(),
            Type = new ObjectTypeValue(functionMember.Identification.Identifier, ValueType.Function),
            Property = null
        };
    }
    
    public static TypeLanguageVariableMemberInstance CreateVariableMemberByProperty(PropertyInfo property,
        ProgramContext programContext, object objectTarget, ref List<ITypeInstance> types)
    {
        var type = GetObjectTypeByType(property.PropertyType, programContext, objectTarget, out var implementModule);
        
        if (implementModule is not null)
        {
            types.AddRange(implementModule.Types);
        }

        return new TypeLanguageVariableMemberInstance
        {
            Parent = property.DeclaringType.Name,
            Module = property.DeclaringType.Assembly.GetName().Name,
            Identification = new KeyTypeMemberIdentification
            {
                Identifier = property.Name
            },
            GetBind = CreateBasedOnFunction.GetBindProperty,
            SetBind = CreateBasedOnFunction.SetBindProperty,
            Type = type,
            Property = property
        };
    }

    public static TypeLanguageFunctionMemberInstance CreateFunctionMemberByMethod(MethodInfo method, ProgramContext programContext, object objectTarget, 
        ref List<ITypeInstance> types)
    {
        var returnType = GetObjectTypeByType(method.ReturnType, programContext, objectTarget, out var implementModule);
            
        if (implementModule is not null)
        {
            types.AddRange(implementModule.Types);
        }

        var parameters = method.GetParameters();
        var arguments = new FunctionArgument[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var parameterType = GetObjectTypeByType(parameter.ParameterType, programContext, objectTarget, out implementModule);
                
            if (implementModule is not null)
            {
                types.AddRange(implementModule.Types);
            }

            arguments[i] = new FunctionArgument(parameter.Name, parameterType);
        }

        return new TypeLanguageFunctionMemberInstance
        {
            Parent = method.DeclaringType.Name,
            Module = method.DeclaringType.Assembly.GetName().Name,
            Identification = new FunctionTypeMemberIdentification
            {
                Identifier = method.Name
            },
            Return = returnType,
            IsAsync = method.GetCustomAttribute(asyncAttribute) is not null,
            Arguments = arguments,
            Bind = CreateBasedOnFunction.ExecuteBindMethod,
            Method = method,
            Access = AccessType.Bindable | AccessType.Static
        };
    }
}