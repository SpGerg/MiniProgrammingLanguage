using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public abstract class AbstractEvaluableExpression : AbstractExpression, IEvaluableExpression
{
    protected AbstractEvaluableExpression(Location location) : base(location)
    {
    }

    public virtual bool IsValue { get; }

    public abstract AbstractValue Evaluate(ProgramContext programContext);
}