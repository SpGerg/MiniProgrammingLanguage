using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class FunctionDeclarationExpression : AbstractEvaluableExpression, IStatement
{
    public FunctionDeclarationExpression(string name, FunctionArgument[] arguments, FunctionBodyExpression body, ObjectTypeValue returnObjectType, bool isAsync, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Arguments = arguments;
        Body = body;
        ReturnObject = returnObjectType;
        IsAsync = isAsync;
        Root = root;
    }

    public string Name { get; }
    
    public FunctionArgument[] Arguments { get; }
    
    public FunctionBodyExpression Body { get; }
    
    public ObjectTypeValue ReturnObject { get; }
    
    public bool IsAsync { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var functionInstance = new UserFunctionInstance
        {
            Name = Name,
            Arguments = Arguments,
            Body = Body,
            Return = ReturnObject,
            IsAsync = IsAsync,
            Root = Root
        };
        
        programContext.Functions.AddOrSet(programContext, functionInstance, Location);

        return null;
    }
}