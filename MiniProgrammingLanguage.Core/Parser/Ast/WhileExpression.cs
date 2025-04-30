namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class WhileExpression : LoopExpression
{
    public WhileExpression(AbstractEvaluableExpression condition, FunctionBodyExpression body, Location location) :
        base(body, location)
    {
        Condition = condition;
    }

    public override bool IsContinue
    {
        get
        {
            var condition = Condition.Evaluate(ProgramContext);

            return condition.AsBoolean(ProgramContext, Location) && !Body.IsEnded;
        }
    }

    public AbstractEvaluableExpression Condition { get; }
}