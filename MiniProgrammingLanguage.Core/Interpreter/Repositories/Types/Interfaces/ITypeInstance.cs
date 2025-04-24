using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeInstance : IInstance
{
    IReadOnlyList<ITypeMember> Members { get; }

    ITypeMember Get(ITypeMemberIdentification identification);

    TypeValue Create();
}