using System.Threading;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionExecuteContext
{
    public LanguageFunctionExecuteContext(FunctionExecuteContext context, AbstractValue[] arguments)
    {
        ProgramContext = context.ProgramContext;
        ArgumentsExpressions = context.Arguments;
        Arguments = arguments;
        Location = context.Location;
        TokenSource = context.TokenSource;
    }

    public ProgramContext ProgramContext { get; }
    
    public AbstractEvaluableExpression[] ArgumentsExpressions { get;}
    
    public AbstractValue[] Arguments { get;}

    public CancellationToken Token => TokenSource.Token;
    
    public Location Location { get; }
    
    internal CancellationTokenSource TokenSource { get; }
}