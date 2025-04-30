using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeMemberValue : ITypeVariableMemberValue
{
    public required AbstractValue Value { get; set; }

    public required ITypeMember Instance { get; init; }

    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;

    public AbstractValue GetValue(TypeMemberGetterContext getterContext)
    {
        return Value;
    }

    public void SetValue(TypeMemberSetterContext getterContext)
    {
        Value = getterContext.Value;
    }
}