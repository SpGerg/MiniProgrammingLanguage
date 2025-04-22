using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeMember
{
    string Parent { get; }
    
    ITypeMemberIdentification Identification { get; }
    
    AbstractValue Default { get; }

    bool IsReadonly { get; }
    
    ObjectTypeValue Type { get; }
}