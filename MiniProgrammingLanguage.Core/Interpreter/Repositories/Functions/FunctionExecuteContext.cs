using System.Threading;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class FunctionExecuteContext
{
    public required ProgramContext ProgramContext { get; init; }

    public required AbstractEvaluableExpression[] Arguments { get; init; }

    public required FunctionBodyExpression Root { get; init; }

    public Location Location { get; init; }

    public CancellationToken Token => TokenSource.Token;

    internal CancellationTokenSource TokenSource { get; init; } = new();
}