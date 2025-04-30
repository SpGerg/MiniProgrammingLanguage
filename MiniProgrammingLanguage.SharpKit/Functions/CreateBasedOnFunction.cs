using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.SharpKit.Exceptions;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class CreateBasedOnFunction
{
    private static string IgnoreCaseAttribute = "sharp_kit_ignore_case";
    
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("create_based_on")
            .SetBind(CreatedBasedOn)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(new FunctionArgument("type_instance", ObjectTypeValue.Object), new FunctionArgument("cs_type", ObjectTypeValue.CSharpObject))
            .Build();
    }

    public static AbstractValue CreatedBasedOn(LanguageFunctionExecuteContext context)
    {
        var typeInstance = context.Arguments.FirstOrDefault();

        if (typeInstance is not TypeValue typeValue)  
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), typeInstance.ToString(), context.Location);

            return null;
        }
        
        var csType = context.Arguments[1];

        var csObject = ((CSharpObjectValue) csType).Value;
        var csObjectType = csObject.GetType();
        
        if (typeInstance.Name != csObjectType.Name)
        {
            throw new NotSameNameException(typeInstance.Name, csObjectType.Name, context.Location);
        }
        
        var properties = csObjectType.GetProperties();
        var methods = csObjectType.GetMethods();
        
        foreach (var member in typeValue.Value.Members)
        {
            var isIgnoreCase = member.Attributes.Contains(IgnoreCaseAttribute);
            
            if (member is ITypeLanguageVariableMember variableMember)
            {
                var csProperty = properties.FirstOrDefault(property => IsNameEquals(property, variableMember.Identification, isIgnoreCase));

                if (csProperty is null)
                {
                    continue;
                }

                variableMember.Type = TypesFactory.GetObjectTypeByType(csProperty.PropertyType, context.ProgramContext, csObject, out var implementModule);
                variableMember.Property = csProperty;
                
                context.ProgramContext.Import(implementModule);
            
                BindProperty(variableMember);
                
                continue;
            }

            if (member is ITypeLanguageFunctionMember functionMember)
            {
                var csMethod = methods.FirstOrDefault(method => IsNameEquals(method, functionMember.Identification, isIgnoreCase));

                if (csMethod is null)
                {
                    continue;
                }

                functionMember.Method = csMethod;

                BindMethod(functionMember);
            }
        }

        typeValue.ObjectTarget = csObject;
        typeValue.Value.Type = csObjectType;

        return typeInstance;
    }
    
    public static AbstractValue ExecuteBindMethod(TypeFunctionExecuteContext context)
    {
        if (context.Member is not ITypeLanguageFunctionMember functionMember)
        {
            return new NoneValue();
        }
        
        var arguments = new object[context.Arguments.Length];

        for (var i = 0; i < context.Arguments.Length; i++)
        {
            var argument = context.Arguments[i];
            
            arguments[i] = TypesFactory.Create(argument.Evaluate(context.ProgramContext), context.ProgramContext);
        }

        var parameters = functionMember.Method.GetParameters();
        
        if (parameters.Length > arguments.Length)
        {
            InterpreterThrowHelper.ThrowArgumentExceptedException(parameters[arguments.Length].Name, context.Location);
        }
        
        var result = functionMember.Method.Invoke(context.Type.ObjectTarget, arguments);

        if (functionMember.Method.ReturnType == typeof(void))
        {
            return new VoidValue();
        }
        
        var type = TypesFactory.Create(result, context.ProgramContext, out var implementModule);
        context.ProgramContext.Import(implementModule);

        return type;
    }

    private static void BindProperty(ITypeLanguageVariableMember variableMember)
    {
        variableMember.GetBind = GetBindProperty;
        variableMember.SetBind = SetBindProperty;
    }
    
    private static void BindMethod(ITypeLanguageFunctionMember functionMember)
    {
        functionMember.Bind = ExecuteBindMethod;
    }

    public static AbstractValue GetBindProperty(TypeMemberGetterContext context)
    {
        if (context.Member is not ITypeLanguageVariableMember variableMember)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(context.Type.Name, context.Member.Identification.Identifier, context.Location);

            return null;
        }
        
        var result = variableMember.Property.GetValue(context.Type.ObjectTarget);
        var type = TypesFactory.Create(result, context.ProgramContext, out var implementModule);
        context.ProgramContext.Import(implementModule);

        return type;
    }
    
    public static void SetBindProperty(TypeMemberSetterContext context)
    {
        if (context.Member is not ITypeLanguageVariableMember variableMember)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(context.Type.Name, context.Member.Identification.Identifier, context.Location);

            return;
        }
        
        var value = TypesFactory.Create(context.Value, context.ProgramContext);
        variableMember.Property.SetValue(context.Type.ObjectTarget, value);
    }

    private static bool IsNameEquals(MemberInfo memberInfo, ITypeMemberIdentification identification, bool isIgnoreCase)
    {
        if (isIgnoreCase)
        {
            return memberInfo.Name.Equals(identification.Identifier, StringComparison.OrdinalIgnoreCase);
        }

        return memberInfo.Name == identification.Identifier;
    }
}