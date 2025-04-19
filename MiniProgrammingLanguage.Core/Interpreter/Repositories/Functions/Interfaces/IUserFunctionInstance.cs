using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;

public interface IUserFunctionInstance : IFunctionInstance
{
    FunctionBodyExpression Body { get; set; }
}