using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class CreateExpression : AbstractEvaluableExpression
{
    public CreateExpression(string name, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Root = root;
    }
    
    public string Name { get; }
    
    public FunctionBodyExpression Root { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var type = programContext.Types.Get(Root, Name, programContext.Module, Location);

        if (type is null)
        {
            InterpreterThrowHelper.ThrowTypeNotFoundException(Name, Location);
        }

        return type.Create();
    }
}