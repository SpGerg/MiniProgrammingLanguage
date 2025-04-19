using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ModuleExpression : AbstractEvaluableExpression, IStatement
{
    public ModuleExpression(StringExpression stringExpression, Location location) : base(location)
    {
        StringExpression = stringExpression;
    }

    public StringExpression StringExpression { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        programContext.Module = StringExpression.Value;

        return new VoidValue();
    }
}