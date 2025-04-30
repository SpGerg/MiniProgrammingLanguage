using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class GetBaseFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("get_base")
            .SetBind(GetBase)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(new FunctionArgument("name", ObjectTypeValue.Object))
            .Build();
    }

    private static AbstractValue GetBase(LanguageFunctionExecuteContext context)
    {
        var content = context.Arguments.FirstOrDefault();

        var typeValue = (TypeValue) content;

        if (typeValue.ObjectTarget is null)
        {
            return new NoneValue();
        }

        return new CSharpObjectValue(typeValue.ObjectTarget);
    }
}