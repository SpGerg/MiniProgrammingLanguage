using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeFunctionExecuteContext : FunctionExecuteContext
{
    public required TypeValue Type { get; init; }
    
    public required ITypeMember Member { get; init; }
}