using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Functions;

public static class SleepFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("sleep")
            .SetModule(StdModule.Name)
            .SetBind(Sleep)
            .SetAccess(AccessType.Static)
            .SetArguments(new FunctionArgument("milliseconds", ObjectTypeValue.RoundNumber))
            .SetReturn(ObjectTypeValue.Void)
            .Build();
    }
    
    private static AbstractValue Sleep(FunctionExecuteContext functionExecuteContext)
    {
        var milliseconds = functionExecuteContext.Arguments[0];
        var time = milliseconds.Evaluate(functionExecuteContext.ProgramContext)
            .AsRoundNumber(functionExecuteContext.ProgramContext, functionExecuteContext.Location);

        functionExecuteContext.Token.WaitHandle.WaitOne(time);

        return new VoidValue();
    }
}