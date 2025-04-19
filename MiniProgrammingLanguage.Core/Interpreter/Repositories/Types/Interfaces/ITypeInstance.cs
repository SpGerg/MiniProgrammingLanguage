using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeInstance : IRepositoryInstance
{
    IReadOnlyList<ITypeMember> Members { get; }

    TypeValue Create(IReadOnlyDictionary<ITypeMemberIdentification, AbstractValue> values = null);
}