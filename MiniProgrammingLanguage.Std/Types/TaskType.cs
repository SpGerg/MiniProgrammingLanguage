using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Types;

public static class TaskType
{
    public static UserTypeInstance Create()
    {
        var id = new TypeVariableMemberInstanceBuilder()
            .SetParent("__task")
            .SetModule(StdModule.Name)
            .SetIdentification(new KeyTypeMemberIdentification
            {
                Identifier = "id"
            })
            .SetAccess(AccessType.Static)
            .SetDefault(new RoundNumberValue(-1))
            .SetType(ObjectTypeValue.RoundNumber)
            .Build();

        return new UserTypeInstanceBuilder()
            .SetName("__task")
            .SetModule(StdModule.Name)
            .SetMembers(id)
            .Build();
    }
}