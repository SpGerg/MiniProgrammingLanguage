using System;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Functions;

public static class TypeofFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("typeof")
            .SetModule(StdModule.Name)
            .SetBind(Typeof)
            .SetAccess(AccessType.Static)
            .SetArguments(new FunctionArgument("type", ObjectTypeValue.String))
            .SetReturn(ObjectTypeValue.TypeInstance)
            .Build();
    }
    
    public static AbstractValue Typeof(LanguageFunctionExecuteContext context)
    {
        var name = context.Arguments.FirstOrDefault().AsString(context.ProgramContext, context.Location);
        var type = context.ProgramContext.Types.Get(context.Root, name, context.ProgramContext.Module, context.Location);

        if (type is null)
        {
            InterpreterThrowHelper.ThrowTypeNotFoundException(name, context.Location);
        }
        
        return new TypeInstanceValue(type);
    }
}