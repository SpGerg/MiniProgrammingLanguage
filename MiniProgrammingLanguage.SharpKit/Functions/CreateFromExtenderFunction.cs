using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class CreateFromExtenderFunction
{
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("create_from_extender")
            .SetBind(CreateFromExtender)
            .SetReturn(ObjectTypeValue.Object)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetArguments(new FunctionArgument("cs_base_type", ObjectTypeValue.TypeInstance),
                new FunctionArgument("cs_extender_type", ObjectTypeValue.CSharpObject)
            )
            .Build();
    }

    public static AbstractValue CreateFromExtender(LanguageFunctionExecuteContext context)
    {
        var baseArgument = (TypeInstanceValue) context.Arguments[0];
        var baseType = baseArgument.Value;
        
        var extenderArgument = (CSharpObjectValue) context.Arguments[1];
        var extenderType = extenderArgument.Value.GetType();

        var typeInstance = context.ProgramContext.Types.Get(context.Root, baseType.Name, context.ProgramContext.Module, context.Location);

        if (typeInstance is null)
        {
            InterpreterThrowHelper.ThrowCannotAccessException(baseType.Name, context.Location);
        }

        var type = baseType.Create();
        type.ObjectTarget = Activator.CreateInstance(extenderType);

        return type;
    }
}