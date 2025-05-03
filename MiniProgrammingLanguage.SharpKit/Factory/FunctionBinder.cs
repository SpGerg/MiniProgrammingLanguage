using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class FunctionBinder
{
    public static void BindMethod(ITypeLanguageFunctionMember functionMember)
    {
        functionMember.Bind = ExecuteBindMethod;
    }
    
    public static void GetBindStaticFunction(LanguageFunctionInstance instance, MethodInfo method)
    {
        var parameters = method.GetParameters();
        
        instance.Bind = context =>
        {
            var arguments = new object[context.Arguments.Length];

            for (var i = 0; i < context.Arguments.Length; i++)
            {
                arguments[i] = TypesFactory.Create(context.Arguments[i]);
            }

            if (parameters.Length > arguments.Length)
            {
                InterpreterThrowHelper.ThrowArgumentExceptedException(parameters[arguments.Length].Name, context.Location);
            }
        
            var result = method.Invoke(null, arguments);

            if (method.ReturnType == typeof(void))
            {
                return new VoidValue();
            }
        
            var type = TypesFactory.Create(result, context.ProgramContext, out var implementModule);
            context.ProgramContext.Import(implementModule);

            return type;
        };
    }
    
    public static AbstractValue ExecuteBindMethod(TypeFunctionExecuteContext context)
    {
        if (context.Member is not ITypeLanguageFunctionMember functionMember)
        {
            return new NoneValue();
        }
        
        if (context.Type.ObjectTarget.GetType().GetMethod(functionMember.Method.Name) is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(functionMember.Method.Name, context.Location);
        }
        
        var arguments = new object[context.Arguments.Length];

        for (var i = 0; i < context.Arguments.Length; i++)
        {
            var argument = context.Arguments[i];
            
            arguments[i] = TypesFactory.Create(argument.Evaluate(context.ProgramContext));
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
}