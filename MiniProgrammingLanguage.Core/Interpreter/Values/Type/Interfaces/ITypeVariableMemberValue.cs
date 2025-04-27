using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

public interface ITypeVariableMemberValue : ITypeMemberValue
{
    AbstractValue GetValue(TypeMemberGetterContext getterContext);
    
    void SetValue(TypeMemberSetterContext setterContext);
}