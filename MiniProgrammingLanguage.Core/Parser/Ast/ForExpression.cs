using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ForExpression : LoopExpression
{
    public ForExpression(AbstractEvaluableExpression condition, AbstractEvaluableExpression variable, BinaryExpression step, string name, FunctionBodyExpression body, Location location) : base(body, location)
    {
        Condition = condition;
        Variable = variable;
        Step = step;
        Name = name;
    }
    
    public AbstractEvaluableExpression Condition { get; }

    public AbstractEvaluableExpression Variable { get; }

    public BinaryExpression Step { get; }
    
    public string Name { get; }

    public override bool IsContinue
    {
        get
        {
            var condition = Condition.Evaluate(ProgramContext);
            
            return condition.AsBoolean(ProgramContext, Condition.Location) && !Body.IsEnded;
        }
    }

    public override void OnLoopStarted()
    {
        if (Variable is AssignExpression)
        {
            Variable.Evaluate(ProgramContext);
            
            return;
        }

        ProgramContext.Variables.AddOrSet(ProgramContext, new UserVariableInstance
        {
            Name = Name,
            Value = new NumberValue(0), 
            Root = Body
        }, Location);
    }
    
    public override void OnIteration()
    {
        ProgramContext.Variables.Set(ProgramContext, new UserVariableInstance
        {
            Name = Name,
            Value = Step.Evaluate(ProgramContext), 
            Root = Body
        }, Location);
    }
}