using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;

public interface IFunctionInstance : IRepositoryInstance
{
    FunctionArgument[] Arguments { get; }
    
    ObjectTypeValue Return { get; }
    
    bool IsAsync { get; }
    
    AbstractValue Evaluate(FunctionExecuteContext context);
}