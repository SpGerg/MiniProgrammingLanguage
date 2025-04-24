using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Std.Functions;
using MiniProgrammingLanguage.Std.Types;
using MiniProgrammingLanguage.Std.Variables;

namespace MiniProgrammingLanguage.Std;

public static class StdModule
{
    private const string Name = "std";
    
    public static ImplementModule Create()
    {
        var print = PrintFunction.Create();
        var sleep = SleepFunction.Create();

        var task = TaskType.Create();

        var module = ModuleVariable.Create();

        return new ImplementModule
        {
            Name = Name,
            Types = new[] { task },
            Functions = new[] { print, sleep },
            Variables = new[] { module },
            Enums = Array.Empty<IEnumInstance>()
        };
    }
}