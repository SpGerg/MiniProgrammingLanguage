using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Variables;

public static class ModuleVariable
{
    public static LanguageVariableInstance Create()
    {
        return new LanguageVariableInstanceBuilder()
            .SetName("__module")
            .SetModule(StdModule.Name)
            .SetGetBindFunc(GetModule)
            .SetAccess(AccessType.Static)
            .SetType(ObjectTypeValue.String)
            .Build();
    }
    
    public static AbstractValue GetModule(VariableGetterContext context)
    {
        return new StringValue(context.ProgramContext.Module);
    }
}