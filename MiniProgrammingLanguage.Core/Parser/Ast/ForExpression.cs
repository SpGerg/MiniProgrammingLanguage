using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ForExpression : LoopExpression
{
    public ForExpression(AbstractEvaluableExpression condition, AbstractEvaluableExpression variable,
        BinaryExpression step, string name, FunctionBodyExpression body, Location location) : base(body, location)
    {
        Condition = condition;
        Variable = variable;
        Step = step;
        Name = name;

        _isAssign = Variable is AssignExpression;
    }

    public AbstractEvaluableExpression Condition { get; }

    public AbstractEvaluableExpression Variable { get; }

    public BinaryExpression Step { get; }

    public string Name { get; }

    private readonly bool _isAssign;

    private UserVariableInstance _userVariableInstance;

    public override bool IsContinue
    {
        get
        {
            var condition = Condition.Evaluate(ProgramContext);

            return condition.AsBoolean(ProgramContext, Location);
        }
    }

    public override void OnLoopStarted()
    {
        if (_isAssign)
        {
            Variable.Evaluate(ProgramContext);

            return;
        }

        _userVariableInstance = new UserVariableInstance
        {
            Name = Name,
            Module = ProgramContext.Module,
            Value = new NumberValue(0),
            Root = Body
        };

        ProgramContext.Variables.Add(_userVariableInstance, Location, false);
    }

    public override void OnIteration()
    {
        _userVariableInstance.Value = Step.Evaluate(ProgramContext);
    }
}