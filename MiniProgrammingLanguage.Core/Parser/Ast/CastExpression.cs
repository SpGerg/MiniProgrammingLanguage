using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class CastExpression : AbstractEvaluableExpression
{
    public CastExpression(ObjectTypeValue objectTypeValue, AbstractEvaluableExpression evaluableExpression,
        Location location) : base(location)
    {
        ObjectTypeValue = objectTypeValue;
        EvaluableExpression = evaluableExpression;
    }

    public ObjectTypeValue ObjectTypeValue { get; }

    public AbstractEvaluableExpression EvaluableExpression { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var result = EvaluableExpression.Evaluate(programContext);
        var casted = result.Cast(programContext, ObjectTypeValue.ValueType, Location);

        if (casted is null)
        {
            InterpreterThrowHelper.ThrowCannotCastException(result.Type.ToString(),
                ObjectTypeValue.ValueType.ToString(), Location);
        }

        return casted;
    }
}