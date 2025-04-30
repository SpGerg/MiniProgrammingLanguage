using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeMember
{
    string Parent { get; }
    
    string Module { get; }

    ITypeMemberIdentification Identification { get; }
    
    IEnumerable<string> Attributes { get; }

    AccessType Access { get; }

    AbstractValue Default { get; }
    
    ObjectTypeValue Type { get; }
}