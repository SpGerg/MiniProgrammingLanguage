using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ReturnExpression : AbstractEvaluableExpression, IControlFlowStatement
{
    public ReturnExpression(AbstractEvaluableExpression evaluableExpression, Location location) : base(location)
    {
        EvaluableExpression = evaluableExpression;
    }
    
    public AbstractEvaluableExpression EvaluableExpression { get; }
    
    public StateType State { get; private set; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        State = StateType.Stopped;

        return EvaluableExpression.Evaluate(programContext);
    }
}