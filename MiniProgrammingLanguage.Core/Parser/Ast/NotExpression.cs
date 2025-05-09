using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class NotExpression : AbstractEvaluableExpression
{
    public NotExpression(AbstractEvaluableExpression condition, Location location) : base(location)
    {
        Condition = condition;
    }
    
    public AbstractEvaluableExpression Condition { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new BooleanValue(!Condition.Evaluate(programContext).AsBoolean(programContext, Location));
    }
}