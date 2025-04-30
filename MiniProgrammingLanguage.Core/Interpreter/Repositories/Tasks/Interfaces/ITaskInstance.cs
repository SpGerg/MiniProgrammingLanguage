using System.Threading;
using System.Threading.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;

public interface ITaskInstance
{
    Task<AbstractValue> Task { get; }

    CancellationTokenSource Token { get; }
}