using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

public interface ITypeMemberExpression
{
    IEnumerable<string> Attributes { get; }
    
    AccessType Access { get; }

    ITypeMember Create(string module);
}