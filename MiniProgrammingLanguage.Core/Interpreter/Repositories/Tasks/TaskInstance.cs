using System.Threading;
using System.Threading.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;

public class TaskInstance : ITaskInstance
{
    public required Task<AbstractValue> Task { get; init; }

    public required CancellationTokenSource Token { get; init; }
}