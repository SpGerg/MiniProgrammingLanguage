using System;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class CreateTypeFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("create_type")
            .SetBind(Create)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(new FunctionArgument("name", ObjectTypeValue.String))
            .Build();
    }

    public static AbstractValue Create(LanguageFunctionExecuteContext context)
    {
        var argument = context.Arguments.FirstOrDefault();
        var content = argument.AsString(context.ProgramContext, context.Location);

        Type csType = null;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            csType = assembly.GetType(content);

            if (csType is null)
            {
                continue;
            }
            
            break;
        }

        if (csType is null)
        {
            InterpreterThrowHelper.ThrowTypeNotFoundException(content, context.Location);
        }

        var instance = Activator.CreateInstance(csType);

        if (instance is null)
        {
            InterpreterThrowHelper.ThrowCannotAccessException(content, context.Location);
        }

        return new CSharpObjectValue(instance);
    }
}