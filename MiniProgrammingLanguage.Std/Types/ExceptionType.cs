using System;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Std.Types;

public class ExceptionType
{
    public static UserTypeInstance Create()
    {
        var id = new TypeVariableMemberInstanceBuilder()
            .SetParent("__exception")
            .SetName("name")
            .SetModule(StdModule.Name)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetDefault(new StringValue(string.Empty))
            .SetType(ObjectTypeValue.String)
            .Build();
        
        var message = new TypeVariableMemberInstanceBuilder()
            .SetParent("__exception")
            .SetName("message")
            .SetModule(StdModule.Name)
            .SetAccess(AccessType.Static | AccessType.ReadOnly)
            .SetDefault(new StringValue(string.Empty))
            .SetType(ObjectTypeValue.String)
            .Build();

        return new UserTypeInstanceBuilder()
            .SetName("__task")
            .SetModule(StdModule.Name)
            .AddMember(id)
            .AddMember(message)
            .Build();
    }
}