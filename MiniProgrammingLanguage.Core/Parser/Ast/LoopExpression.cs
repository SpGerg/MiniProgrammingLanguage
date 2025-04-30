using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public abstract class LoopExpression : AbstractEvaluableExpression, IStatement
{
    public LoopExpression(FunctionBodyExpression body, Location location) : base(location)
    {
        Body = body;
    }

    public abstract bool IsContinue { get; }

    public FunctionBodyExpression Body { get; }

    public ProgramContext ProgramContext { get; private set; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        ProgramContext = programContext;

        OnLoopStarted();

        AbstractValue result = new VoidValue();

        while (IsContinue)
        {
            result = Body.Evaluate(programContext);

            OnIteration();

            if (!Body.IsEnded)
            {
                continue;
            }

            break;
        }

        programContext.Clear(Body);

        return result;
    }

    public virtual void OnLoopStarted()
    {
    }

    public virtual void OnIteration()
    {
    }
}