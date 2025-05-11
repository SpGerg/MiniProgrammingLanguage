using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class TableValue : TypeValue
{
    public TableValue(ITypeInstance value, IReadOnlyDictionary<ITypeMemberIdentification, ITypeMemberValue> members) : base(value, members)
    {
    }

    public TableValue(ITypeInstance value) : base(value)
    {
    }
}