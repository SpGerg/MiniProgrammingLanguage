using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

public interface ITypeFunctionMemberValue : ITypeMemberValue
{
    AbstractValue GetValue(TypeFunctionExecuteContext context);
}