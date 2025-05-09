using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Std.Functions;
using MiniProgrammingLanguage.Std.Types;
using MiniProgrammingLanguage.Std.Variables;

namespace MiniProgrammingLanguage.Std;

public static class StdModule
{
    /// <summary>
    /// Name
    /// </summary>
    public const string Name = "std";
    
    /// <summary>
    /// Create std implement module
    /// </summary>
    /// <returns></returns>
    public static ImplementModule Create()
    {
        var print = PrintFunction.Create();
        var sleep = SleepFunction.Create();
        var typeofFunction = TypeofFunction.Create();

        var task = TaskType.Create();
        var exception = ExceptionType.Create();

        var module = ModuleVariable.Create();

        return new ImplementModule
        {
            Name = Name,
            Types = new[] { task, exception },
            Functions = new[] { print, sleep, typeofFunction },
            Variables = new[] { module },
            Enums = Array.Empty<IEnumInstance>()
        };
    }
}