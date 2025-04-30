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
            .SetArguments(new FunctionArgument("type_instance", ObjectTypeValue.TypeInstance), new FunctionArgument("cs_type", ObjectTypeValue.CSharpObject))
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
        var csObjectType = csObject.GetType();
        
        if (typeInstance.Name != csObjectType.Name)
        {
            throw new NotSameNameException(typeInstance.Name, csObjectType.Name, context.Location);
        }
        
        var properties = csObjectType.GetProperties();
        var methods = csObjectType.GetMethods();
        
        foreach (var member in type.Value.Members)
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
            
                PropertyBinder.BindProperty(variableMember);
                
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

                MethodBinder.BindMethod(functionMember);
            }
        }
        
        type.Value.Type = csObjectType;

        return new VoidValue();
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