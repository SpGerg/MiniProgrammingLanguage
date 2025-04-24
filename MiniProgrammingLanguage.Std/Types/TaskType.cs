using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Std.Types;

public static class TaskType
{
    public static UserTypeInstance Create()
    {
        var id = new TypeVariableMemberInstanceBuilder()
            .SetParent("__task")
            .SetIdentification(new KeyTypeMemberIdentification
            {
                Identifier = "id"
            })
            .SetDefault(new RoundNumberValue(-1))
            .SetReadOnly(true)
            .SetType(ObjectTypeValue.RoundNumber)
            .Build();

        return new UserTypeInstanceBuilder()
            .SetName("__task")
            .SetMembers(id)
            .Build();
    }
}