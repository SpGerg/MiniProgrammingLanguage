using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeMemberGetterContext
{
    public required ProgramContext ProgramContext { get; init; }

    public required TypeValue Type { get; init; }

    public required ITypeMember Member { get; init; }

    public Location Location { get; init; } = Location.Default;
}