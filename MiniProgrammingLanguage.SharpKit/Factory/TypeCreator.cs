using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

public static class TypeCreator
{
    public static readonly Type AsyncAttribute = typeof(AsyncStateMachineAttribute);
    
    public static TypeValue Create(Type target, ProgramContext programContext, object objectTarget, out ImplementModule implementModule)
    {
        var types = new List<ITypeInstance>();
        var type = CreateClassOrStructure(target, programContext, objectTarget, types);
        
        implementModule = new ImplementModule
        {
            Name = "global",
            Types = types
        };

        return type;
    }

    private static TypeValue CreateClassOrStructure(Type type, ProgramContext programContext, object objectTarget,
        List<ITypeInstance> types)
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
        var type = TypesFactory.GetObjectTypeByType(property.PropertyType, programContext, objectTarget, out var implementModule);
        
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
            GetBind = PropertyBinder.GetBindProperty,
            SetBind = PropertyBinder.SetBindProperty,
            Type = type,
            Property = property
        };
    }

    public static TypeLanguageFunctionMemberInstance CreateFunctionMemberByMethod(MethodInfo method, ProgramContext programContext, object objectTarget, 
        ref List<ITypeInstance> types)
    {
        var returnType = TypesFactory.GetObjectTypeByType(method.ReturnType, programContext, objectTarget, out var implementModule);
            
        if (implementModule is not null)
        {
            types.AddRange(implementModule.Types);
        }

        var parameters = method.GetParameters();
        var arguments = new FunctionArgument[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var parameterType = TypesFactory.GetObjectTypeByType(parameter.ParameterType, programContext, objectTarget, out implementModule);
                
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
            IsAsync = method.GetCustomAttribute(AsyncAttribute) is not null,
            Arguments = arguments,
            Bind = FunctionBinder.ExecuteBindMethod,
            Method = method,
            Access = AccessType.Bindable | AccessType.Static
        };
    }
}