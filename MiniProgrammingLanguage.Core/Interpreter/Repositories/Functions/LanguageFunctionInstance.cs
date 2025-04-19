using System;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionInstance : IFunctionInstance, ILanguageInstance
{
    public required string Name { get; init; }
    
    public required FunctionBodyExpression Root { get; init; }

    public required Func<FunctionExecuteContext, AbstractValue> Bind { get; init; }

    public required FunctionArgument[] Arguments { get; init; }
    
    public required ObjectTypeValue Return { get; init; }
    
    public required bool IsAsync { get; init; }

    public AbstractValue Evaluate(FunctionExecuteContext context)
    {
        var result = Bind.Invoke(context);

        if (!Return.Is(result))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name, Return.AsString(context.ProgramContext, context.Location), result.Type.ToString(), context.Location);
        }

        return result;
    }
    
    public bool TryChange(ProgramContext programContext, IRepositoryInstance repositoryInstance, Location location, out AbstractLanguageException exception)
    {
        exception = new CannotAccessException(Name, location);
        return false;
    }
}