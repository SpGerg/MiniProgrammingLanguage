using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

public interface ITypeMemberValue
{
    ObjectTypeValue Type { get; }
    
    ITypeMember Instance { get; }

    AbstractValue GetValue(TypeMemberGetterContext getterContext);
    
    void SetValue(TypeMemberSetterContext setterContext);
}