using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeLanguageFunctionMemberValue : ITypeFunctionMemberValue
{
    public TypeLanguageFunctionMemberValue(ITypeLanguageFunctionMember typeMember)
    {
        _typeMember = typeMember;
        Instance = typeMember;
    }
    
    public required ObjectTypeValue Type { get; init; }
    
    public ITypeMember Instance { get; }
    
    private readonly ITypeLanguageFunctionMember _typeMember;
    
    public AbstractValue GetValue(TypeFunctionExecuteContext context)
    {
        return _typeMember.Bind.Invoke(context);
    }
}