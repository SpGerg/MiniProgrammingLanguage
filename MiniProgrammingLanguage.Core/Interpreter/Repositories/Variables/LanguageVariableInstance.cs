using System;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class LanguageVariableInstance : IVariableInstance, ILanguageInstance
{
    public required string Name { get; init; }

    public required string Module { get; init; }

    public required FunctionBodyExpression Root { get; init; }

    public required Func<VariableGetterContext, AbstractValue> GetBind { get; set; }

    public required ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;

    public Action<VariableSetterContext> SetBind { get; set; }

    public AccessType Access { get; init; } = AccessType.ReadOnly;

    public AbstractValue GetValue(VariableGetterContext context)
    {
        var result = GetBind.Invoke(context);

        if (!Type.Is(result))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name,
                Type.AsString(context.ProgramContext, context.Location), result.Type.ToString(), context.Location);
        }

        return result;
    }

    public bool TryChange(ProgramContext programContext, IInstance instance, Location location,
        out AbstractLanguageException exception)
    {
        if (instance is not IVariableInstance variableInstance)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        if (SetBind is null)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        var getterContext = new VariableGetterContext
        {
            ProgramContext = programContext,
            Location = location
        };

        var context = new VariableSetterContext
        {
            ProgramContext = programContext,
            Value = variableInstance.GetValue(getterContext),
            Location = location
        };

        try
        {
            exception = null;

            SetBind.Invoke(context);
        }
        catch (AbstractLanguageException languageException)
        {
            exception = languageException;
        }

        return true;
    }
}