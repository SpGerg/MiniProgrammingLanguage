using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class FunctionExpression : AbstractEvaluableExpression
{
    public FunctionExpression(FunctionDeclarationExpression functionDeclarationExpression, Location location) : base(location)
    {
        FunctionDeclarationExpression = functionDeclarationExpression;
    }
    
    public FunctionDeclarationExpression FunctionDeclarationExpression { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return new FunctionValue(FunctionDeclarationExpression.Create(programContext.Module));
    }
}