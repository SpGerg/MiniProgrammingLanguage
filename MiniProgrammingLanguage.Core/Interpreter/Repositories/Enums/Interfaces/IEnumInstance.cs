using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;

public interface IEnumInstance : IInstance
{
    IReadOnlyDictionary<string, int> Members { get; }

    bool TryGetByName(string name, out int value);

    bool TryGetByValue(int value, out string name);

    EnumValue Create();
}