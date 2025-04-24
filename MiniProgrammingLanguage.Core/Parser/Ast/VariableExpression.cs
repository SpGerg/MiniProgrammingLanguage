using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class VariableExpression : AbstractEvaluableExpression
{
    public VariableExpression(string name, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Root = root;
    }
    
    public string Name { get; }
    
    public FunctionBodyExpression Root { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var variable = programContext.Variables.Get(Root, Name, programContext.Module, Location);

        if (variable is null)
        {
            InterpreterThrowHelper.ThrowVariableNotFoundException(Name, Location);
        }
        
        return variable.GetValue(new VariableGetterContext
        {
            ProgramContext = programContext,
            Location = Location
        });
    }
}