using System;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionInstance : IFunctionInstance, ILanguageInstance
{
    public required string Name { get; init; }

    public required string Module { get; init; }

    public required FunctionBodyExpression Root { get; init; }

    public required Func<LanguageFunctionExecuteContext, AbstractValue> Bind { get; set; }

    public required FunctionArgument[] Arguments { get; init; }

    public required ObjectTypeValue Return { get; init; }

    public required bool IsAsync { get; init; }

    public AccessType Access { get; init; } = AccessType.ReadOnly;

    public bool IsDeclared => Bind is not null;

    public AbstractValue Evaluate(FunctionExecuteContext context)
    {
        if (!IsDeclared)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(Name, context.Location);
        }

        var arguments = new AbstractValue[Arguments.Length];

        for (var i = 0; i < Arguments.Length; i++)
        {
            FunctionArgument argument;

            if (i < context.Arguments.Length)
            {
                argument = Arguments[i];

                var value = context.Arguments[i].Evaluate(context.ProgramContext);

                if (!argument.Type.Is(value))
                {
                    InterpreterThrowHelper.ThrowIncorrectTypeException(argument.Type.ValueType.ToString(),
                        value.Type.ToString(), context.Location);
                }

                arguments[i] = value;

                continue;
            }

            argument = Arguments[i];

            if (!argument.IsRequired)
            {
                arguments[i] = argument.Default;
                
                continue;
            }

            InterpreterThrowHelper.ThrowArgumentExceptedException(argument.Name, context.Location);
        }

        var result = Bind.Invoke(new LanguageFunctionExecuteContext(context, arguments));

        if (!Return.Is(result))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name,
                Return.AsString(context.ProgramContext, context.Location), result.Type.ToString(), context.Location);
        }

        return result;
    }

    public bool TryChange(ProgramContext programContext, IInstance instance, Location location,
        out AbstractLanguageException exception)
    {
        exception = new CannotAccessException(Name, location);
        return false;
    }

    public FunctionValue Create()
    {
        return new FunctionValue(this);
    }

    public IFunctionInstance Copy(string name = null, FunctionBodyExpression root = null)
    {
        return new LanguageFunctionInstance
        {
            Name = name ?? Name,
            Module = Module,
            Arguments = Arguments,
            Bind = Bind,
            IsAsync = IsAsync,
            Return = Return,
            Root = root ?? Root
        };
    }
}