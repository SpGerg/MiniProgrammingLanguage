using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeMemberSetterContext : TypeMemberGetterContext
{
    public required AbstractValue Value { get; init; }
}