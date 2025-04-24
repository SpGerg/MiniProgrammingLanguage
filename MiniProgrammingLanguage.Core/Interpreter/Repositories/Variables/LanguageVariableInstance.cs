using System;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class LanguageVariableInstance : IVariableInstance, ILanguageInstance
{
    public required string Name { get; init; }
    
    public required FunctionBodyExpression Root { get; init; }
    
    public required Func<VariableGetterContext, AbstractValue> Bind { get; set; }
    
    public required ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;

    public AbstractValue GetValue(VariableGetterContext context)
    {
        var result = Bind.Invoke(context);
        
        if (!Type.Is(result))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name, Type.AsString(context.ProgramContext, context.Location), result.Type.ToString(), context.Location);
        }

        return result;
    }
    
    public bool TryChange(ProgramContext programContext, IRepositoryInstance repositoryInstance, Location location, out AbstractLanguageException exception)
    {
        exception = new CannotAccessException(Name, location);
        return false;
    }
}