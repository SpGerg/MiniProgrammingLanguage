using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;

public interface IInstance
{
    string Name { get; }

    string Module { get; }
    
    AccessType Access { get; }

    FunctionBodyExpression Root { get; }

    bool TryChange(ProgramContext programContext, IInstance instance, Location location, out AbstractLanguageException exception);
}