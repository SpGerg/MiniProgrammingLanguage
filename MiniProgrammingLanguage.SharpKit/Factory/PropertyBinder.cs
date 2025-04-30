using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class PropertyBinder
{
    public static void BindProperty(ITypeLanguageVariableMember variableMember)
    {
        variableMember.GetBind = GetBindProperty;
        variableMember.SetBind = SetBindProperty;
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
        
        var value = TypesFactory.Create(context.Value);
        variableMember.Property.SetValue(context.Type.ObjectTarget, value);
    }
}