using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter;

public class ImplementModule
{
    public required string Name { get; init; }

    public bool IsGlobal => Name is "global";

    public IEnumerable<ITypeInstance> Types { get; init; } = Array.Empty<ITypeInstance>();

    public IEnumerable<IFunctionInstance> Functions { get; init; } = Array.Empty<IFunctionInstance>();

    public IEnumerable<IEnumInstance> Enums { get; init; } = Array.Empty<IEnumInstance>();

    public IEnumerable<IVariableInstance> Variables { get; init; } = Array.Empty<IVariableInstance>();

    public Location Location { get; init; } = Location.Default;
}