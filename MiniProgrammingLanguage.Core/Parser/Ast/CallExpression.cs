using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class CallExpression : AbstractEvaluableExpression
{
    public CallExpression(FunctionDeclarationExpression functionDeclarationExpression, Location location) :
        base(location)
    {
        FunctionDeclarationExpression = functionDeclarationExpression;
        _functionCall =
            new FunctionCallExpression(string.Empty, Array.Empty<AbstractEvaluableExpression>(), null, Location);
    }

    public FunctionDeclarationExpression FunctionDeclarationExpression { get; }

    private readonly FunctionCallExpression _functionCall;

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return _functionCall.Call(programContext, FunctionDeclarationExpression.Create(programContext.Module));
    }
}