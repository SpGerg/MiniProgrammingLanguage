using System;
using System.Linq;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.SharpKit.Exceptions;
using MiniProgrammingLanguage.SharpKit.Factory;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class BindFunction
{
    private const string IgnoreCaseAttribute = "sharp_kit_ignore_case";

    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("bind")
            .SetBind(Bind)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(
                new FunctionArgument("type_instance", ObjectTypeValue.TypeInstance),
                new FunctionArgument("cs_type", ObjectTypeValue.CSharpObject)
                )
            .Build();
    }

    public static AbstractValue Bind(LanguageFunctionExecuteContext context)
    {
        var typeInstance = context.Arguments.FirstOrDefault();

        if (typeInstance is not TypeInstanceValue type)  
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), typeInstance.ToString(), context.Location);

            return null;
        }
        
        var csType = context.Arguments[1];

        var csObject = ((CSharpObjectValue) csType).Value;
        var csObjectType = csObject as Type ?? csObject.GetType();

        var isExtends = false;

        if (type.Value.Type is not null)
        {
            isExtends = IsExtends(csObjectType, type.Value.Type.Name);
        }

        if (typeInstance.Name != csObjectType.Name && !isExtends)
        {
            throw new NotSameNameException(typeInstance.Name, csObjectType.Name, context.Location);
        }

        var properties = csObjectType.GetProperties();
        var methods = csObjectType.GetMethods();

        foreach (var member in type.Value.Members)
        {
            var isIgnoreCase = member.Attributes.Contains(IgnoreCaseAttribute);
            
            switch (member)
            {
                case ITypeLanguageVariableMember variableMember:
                {
                    var csProperty = properties.FirstOrDefault(property => IsNameEquals(property, variableMember.Identification, isIgnoreCase));

                    if (csProperty is null)
                    {
                        continue;
                    }

                    if (isExtends && csProperty.DeclaringType != csObjectType)
                    {
                        continue;
                    }

                    var variableType = TypesFactory.GetObjectTypeByType(csProperty.PropertyType, context.ProgramContext, csObject, out var implementModule);
                    context.ProgramContext.Import(implementModule);
                    
                    if (csProperty.GetAccessors(false)[0].IsStatic)
                    {
                        var variableInstance = new LanguageVariableInstance
                        {
                            Name = $"{csObjectType.Name}_{csProperty.Name}",
                            Module = csObjectType.Assembly.GetName().Name,
                            Type = variableType,
                            Access = AccessType.Static | AccessType.ReadOnly,
                            GetBind = null,
                            Root = null
                        };
                        
                        PropertyBinder.BindStaticProperty(variableInstance, csProperty);
                        context.ProgramContext.Variables.Add(variableInstance);
                        
                        continue;
                    }

                    variableMember.Type = variableType;
                    variableMember.Property ??= csProperty;

                    PropertyBinder.BindProperty(variableMember);
                
                    continue;
                }
                case ITypeLanguageFunctionMember functionMember:
                {
                    var csMethod = methods.FirstOrDefault(method => IsNameEquals(method, functionMember.Identification, isIgnoreCase));

                    if (csMethod is null)
                    {
                        continue;
                    }
                    
                    if (isExtends && csMethod.DeclaringType != csObjectType)
                    {
                        continue;
                    }
                    
                    if (csMethod.IsStatic)
                    {
                        var parameters = csMethod.GetParameters();
                        var arguments = new FunctionArgument[parameters.Length];
                        
                        var functionReturnType = TypesFactory.GetObjectTypeByType(csMethod.ReturnType, context.ProgramContext, csObject, out var returnModule);
                        context.ProgramContext.Import(returnModule);

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            var parameter = parameters[i];
                            var argumentType = TypesFactory.GetObjectTypeByType(parameter.ParameterType, context.ProgramContext, csObject, out var implementModule);
                            
                            context.ProgramContext.Import(implementModule);
                            
                            arguments[i] = new FunctionArgument(parameter.Name, argumentType);
                        }
                        
                        var functionInstance = new LanguageFunctionInstance
                        {
                            Name = $"{csObjectType.Name}_{csMethod.Name}",
                            Module = csObjectType.Assembly.GetName().Name,
                            Arguments = arguments,
                            Return = functionReturnType,
                            IsAsync = csMethod.GetCustomAttribute(TypeCreator.AsyncAttribute) is not null,
                            Bind = null,
                            Access = AccessType.Static | AccessType.ReadOnly,
                            Root = null
                        };
                        
                        FunctionBinder.GetBindStaticFunction(functionInstance, csMethod); 
                        context.ProgramContext.Functions.Add(functionInstance);
                        
                        continue;
                    }

                    functionMember.Method ??= csMethod;

                    FunctionBinder.BindMethod(functionMember);
                    break;
                }
            }
        }

        if (!isExtends)
        {
            type.Value.Type = csObjectType;
        }
        
        return VoidValue.Instance;
    }

    private static bool IsNameEquals(MemberInfo memberInfo, ITypeMemberIdentification identification, bool isIgnoreCase)
    {
        if (isIgnoreCase)
        {
            return memberInfo.Name.Equals(identification.Identifier, StringComparison.OrdinalIgnoreCase);
        }

        return memberInfo.Name == identification.Identifier;
    }

    private static bool IsExtends(Type type, string extender)
    {
        if (type.BaseType is null)
        {
            return false;
        }

        return type.BaseType.Name == extender || IsExtends(type.BaseType, extender);
    }
}