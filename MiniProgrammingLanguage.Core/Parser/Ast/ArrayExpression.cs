using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ArrayExpression : AbstractEvaluableExpression
{
    public ArrayExpression(AbstractEvaluableExpression[] evaluableExpressions, Location location) : base(location)
    {
        EvaluableExpressions = evaluableExpressions;
    }

    public AbstractEvaluableExpression[] EvaluableExpressions { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var values = new AbstractValue[EvaluableExpressions.Length];

        for (var i = 0; i < EvaluableExpressions.Length; i++)
        {
            values[i] = EvaluableExpressions[i].Evaluate(programContext);
        }

        return new ArrayValue(values);
    }
}