using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.SharpKit.Variables;

public static class SharpKitVariable
{
    public static LanguageVariableInstance Create()
    {
        return new LanguageVariableInstanceBuilder()
            .SetName("sharp_kit")
            .SetModule(SharpKitModule.Name)
            .SetGetBindFunc(IsExists)
            .SetAccess(AccessType.ReadOnly)
            .Build();
    }
    
    private static AbstractValue IsExists(VariableGetterContext variableGetterContext)
    {
        return new BooleanValue(true);
    }
}