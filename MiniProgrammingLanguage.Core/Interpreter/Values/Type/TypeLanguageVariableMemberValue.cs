using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeLanguageVariableMemberValue : ITypeVariableMemberValue
{
    public TypeLanguageVariableMemberValue(ITypeLanguageVariableMember typeMember)
    {
        _typeMember = typeMember;
        Instance = typeMember;
    }

    public required ObjectTypeValue Type { get; init; }

    public ITypeMember Instance { get; }

    private readonly ITypeLanguageVariableMember _typeMember;

    public AbstractValue GetValue(TypeMemberGetterContext getterContext)
    {
        if (_typeMember.GetBind is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(_typeMember.Identification.Identifier,
                getterContext.Location);
        }

        return _typeMember.GetBind.Invoke(getterContext);
    }

    public void SetValue(TypeMemberSetterContext setterContext)
    {
        if (_typeMember.SetBind is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(_typeMember.Identification.Identifier,
                setterContext.Location);
        }

        _typeMember.SetBind.Invoke(setterContext);
    }
}