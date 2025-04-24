using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

public interface ITypeMemberExpression
{
    ITypeMember Create(string module);
}