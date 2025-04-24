using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeMember
{
    string Parent { get; }
    
    ITypeMemberIdentification Identification { get; }
    
    AbstractValue Default { get; }

    AccessType Access { get; }
    
    ObjectTypeValue Type { get; }
}