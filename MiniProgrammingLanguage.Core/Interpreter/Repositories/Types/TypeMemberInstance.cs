using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeMemberInstance : ITypeMember
{
    public required string Parent { get; init; }
    
    public required ITypeMemberIdentification Identification { get; init; }
    
    public required AbstractValue Default { get; init;  }
    
    public required bool IsReadonly { get; init;  }
    
    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;
}