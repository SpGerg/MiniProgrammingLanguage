using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeInstance : IInstance
{
    IReadOnlyList<ITypeMember> Members { get; }
    
    Type Type { get; set; }

    ITypeMember Get(ITypeMemberIdentification identification);

    TypeValue Create();
}