using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class UnaryExpression : AbstractEvaluableExpression
{
    public UnaryExpression(BinaryOperatorType operatorType, AbstractEvaluableExpression evaluableExpression,
        Location location) : base(location)
    {
        Operator = operatorType;
        EvaluableExpression = evaluableExpression;
    }

    public BinaryOperatorType Operator { get; }

    public AbstractEvaluableExpression EvaluableExpression { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return Operator switch
        {
            BinaryOperatorType.Plus => EvaluableExpression.Evaluate(programContext),
            BinaryOperatorType.Minus => new NumberValue(
                -EvaluableExpression.Evaluate(programContext).AsNumber(programContext, Location)),
            _ => EvaluableExpression.Evaluate(programContext)
        };
    }
}