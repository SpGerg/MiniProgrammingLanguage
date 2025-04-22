using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
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

    public bool IsAnonymous => Name == string.Empty;
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var result = Create();
        var value = result.Create();
        
        if (!IsAnonymous)
        {
            programContext.Variables.AddOrSet(programContext, new UserVariableInstance
            {
                Name = result.Name,
                ObjectType = ObjectTypeValue.Function,
                Root = result.Root,
                Value = value
            }, Location);
        }

        programContext.Functions.AddOrSet(programContext, Create(), Location);
        
        return value;
    }

    public UserFunctionInstance Create()
    {
        return new UserFunctionInstance
        {
            Name = Name,
            Arguments = Arguments,
            Body = Body,
            Return = ReturnObject,
            IsAsync = IsAsync,
            Root = Root
        };
    }
}