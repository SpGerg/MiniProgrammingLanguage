using System;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Functions;

public static class PrintFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("print")
            .SetModule(StdModule.Name)
            .SetBind(Print)
            .SetAccess(AccessType.Static)
            .SetArguments(new FunctionArgument("content"))
            .SetReturn(ObjectTypeValue.Void)
            .Build();
    }
    
    public static AbstractValue Print(FunctionExecuteContext context)
    {
        var argument = context.Arguments[0];
        var content = argument.Evaluate(context.ProgramContext);

        var message = string.Empty;

        if (content is NoneValue noneValue)
        {
            message = "none";
        }
        else
        {
            message = content.AsString(context.ProgramContext, context.Location);
        }
            
        Console.WriteLine(message);
        
        return new VoidValue();
    }
}