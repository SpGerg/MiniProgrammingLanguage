using System;
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
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class CreateBasedOnFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("create_based_on")
            .SetBind(Include)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(new FunctionArgument("type_instance", ObjectTypeValue.Object), new FunctionArgument("cs_type", ObjectTypeValue.CSharpObject))
            .Build();
    }

    public static AbstractValue Include(FunctionExecuteContext context)
    {
        var typeArgument = context.Arguments.FirstOrDefault();
        var typeInstance = typeArgument.Evaluate(context.ProgramContext);

        if (typeInstance is not TypeValue typeValue)  
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), typeInstance.ToString(), context.Location);

            return null;
        }

        var csTypeArgument = context.Arguments[1];
        var csType = csTypeArgument.Evaluate(context.ProgramContext);

        var csObject = ((CSharpObjectValue) csType).Value;
        var csObjectType = csObject.GetType();
        
        foreach (var member in typeValue.Value.Members)
        {
            if (member is not ITypeLanguageMember languageMember)
            {
                continue;
            }

            var properties = csObjectType.GetProperties();
            var csMember = properties.FirstOrDefault(property => property.Name == member.Identification.Identifier);

            if (csMember is null)
            {
                continue;
            }

            languageMember.Property = csMember;
            
            BindProperty(languageMember);
        }

        typeValue.ObjectTarget = csObject;

        return typeInstance;
    }

    private static void BindProperty(ITypeLanguageMember languageMember)
    {
        languageMember.GetBind = GetBindProperty;
        languageMember.SetBind = SetBindProperty;
    }

    private static AbstractValue GetBindProperty(TypeMemberGetterContext getterContext)
    {
        if (getterContext.Member is not ITypeLanguageMember languageMember)
        {
            return new NoneValue();
        }

        var result = languageMember.Property.GetValue(getterContext.Type.ObjectTarget);

        return TypesFactory.Create(result);
    }
    
    private static void SetBindProperty(TypeMemberSetterContext setterContext)
    {
        if (setterContext.Member is not ITypeLanguageMember languageMember)
        {
            return;
        }

        var value = TypesFactory.Create(setterContext.Value);
        
        languageMember.Property.SetValue(setterContext.Type.ObjectTarget, value);
    }
}