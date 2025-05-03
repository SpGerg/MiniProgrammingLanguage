using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class PropertyBinder
{
    public static void BindProperty(ITypeLanguageVariableMember variableMember)
    {
        variableMember.GetBind = GetBindProperty;
        variableMember.SetBind = SetBindProperty;
    }

    public static void BindStaticProperty(LanguageVariableInstance variableInstance, PropertyInfo propertyInfo)
    {
        variableInstance.GetBind = context =>
        {
            var result = TypesFactory.Create(propertyInfo, context.ProgramContext, out var implementModule);
            context.ProgramContext.Import(implementModule);

            return result;
        };
        variableInstance.SetBind = context =>
        {
            var value = TypesFactory.Create(context.Value, context.ProgramContext);
            propertyInfo.SetValue(null, value);
        };
    }

    public static AbstractValue GetBindProperty(TypeMemberGetterContext context)
    {
        if (context.Member is not ITypeLanguageVariableMember variableMember)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(context.Type.Name, context.Member.Identification.Identifier, context.Location);

            return null;
        }
        
        if (variableMember.Property is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(context.Type.Name, context.Location);
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

        if (variableMember.Property is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(context.Type.Name, context.Location);
        }
        
        var value = TypesFactory.Create(context.Value, context.ProgramContext);
        variableMember.Property.SetValue(context.Type.ObjectTarget, value);
    }
}