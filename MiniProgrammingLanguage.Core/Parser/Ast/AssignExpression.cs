using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignExpression : AbstractEvaluableExpression, IAssignExpression
{
    public AssignExpression(string name, AbstractEvaluableExpression evaluableExpression, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Root = root;
        EvaluableExpression = evaluableExpression;
    }

    public string Name { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public AbstractEvaluableExpression EvaluableExpression { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var value = EvaluableExpression.Evaluate(programContext);
        
        programContext.Variables.AddOrSet(programContext, new UserVariableInstance
        {
            Name = Name,
            Root = Root,
            Value = value
        }, Location);

        return value;
    }
}