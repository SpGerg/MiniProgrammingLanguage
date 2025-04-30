using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class BreakExpression : AbstractEvaluableExpression, IControlFlowStatement
{
    public BreakExpression(Location location) : base(location)
    {
    }

    public StateType State { get; private set; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        State = StateType.Stopped;

        return new VoidValue();
    }
}