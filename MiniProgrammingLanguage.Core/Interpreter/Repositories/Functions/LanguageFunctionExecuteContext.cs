using System.Threading;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionExecuteContext
{
    public LanguageFunctionExecuteContext(FunctionExecuteContext context, AbstractValue[] arguments)
    {
        ProgramContext = context.ProgramContext;
        Root = context.Root;
        ArgumentsExpressions = context.Arguments;
        Arguments = arguments;
        Location = context.Location;
        TokenSource = context.TokenSource;
    }

    public ProgramContext ProgramContext { get; }
    
    public AbstractEvaluableExpression[] ArgumentsExpressions { get;}
    
    public AbstractValue[] Arguments { get;}

    public FunctionBodyExpression Root { get; }

    public Location Location { get; }
    
    public CancellationToken Token => TokenSource.Token;
    
    internal CancellationTokenSource TokenSource { get; }
}