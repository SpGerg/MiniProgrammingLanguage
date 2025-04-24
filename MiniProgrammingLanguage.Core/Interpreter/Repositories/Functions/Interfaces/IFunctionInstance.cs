using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;

public interface IFunctionInstance : IRepositoryInstance
{
    FunctionArgument[] Arguments { get; }
    
    ObjectTypeValue Return { get; }
    
    bool IsAsync { get; }
    
    bool IsDeclared { get; }
    
    AbstractValue Evaluate(FunctionExecuteContext context);

    FunctionValue Create();
    
    IFunctionInstance Copy(string name = null, FunctionBodyExpression root = null);
}