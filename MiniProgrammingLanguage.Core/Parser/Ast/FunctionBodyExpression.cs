using System.Threading;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class FunctionBodyExpression : AbstractEvaluableExpression
{
    public FunctionBodyExpression(IStatement[] statements, FunctionBodyExpression root, Location location) :
        base(location)
    {
        Statements = statements;
        Root = root;
        Token = new CancellationToken();
    }

    public IStatement[] Statements { get; set; }

    public CancellationToken Token { get; set; }

    public FunctionBodyExpression Root { get; }

    public StateType State { get; private set; }

    public bool IsEnded => State is StateType.Stopped;

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var token = Token;

        AbstractValue result = VoidValue.Instance;

        State = StateType.Running;

        foreach (var statement in Statements)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            result = statement.Evaluate(programContext);

            if (statement is not IControlFlowStatement controlFlowStatement)
            {
                continue;
            }

            if (controlFlowStatement.State is StateType.Running)
            {
                continue;
            }

            State = controlFlowStatement.State;
            break;
        }

        return result;
    }
}