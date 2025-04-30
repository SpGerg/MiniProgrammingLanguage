using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class UserFunctionInstance : IFunctionInstance
{
    public required string Name { get; set; }

    public required string Module { get; init; }

    public required FunctionBodyExpression Root { get; set; }

    public required FunctionBodyExpression Body { get; set; }

    public required FunctionArgument[] Arguments { get; init; }

    public required ObjectTypeValue Return { get; init; }

    public required bool IsAsync { get; init; }

    public AccessType Access { get; init; }

    public bool IsDeclared => Body is not null;

    public AbstractValue Evaluate(FunctionExecuteContext context)
    {
        if (!IsDeclared)
        {
            InterpreterThrowHelper.ThrowFunctionNotDeclaredException(Name, context.Location);
        }

        var programContext = context.ProgramContext;

        for (var i = 0; i < Arguments.Length; i++)
        {
            var argument = Arguments[i];
            AbstractValue value;

            if (i > context.Arguments.Length - 1)
            {
                if (argument.IsRequired)
                {
                    InterpreterThrowHelper.ThrowArgumentExceptedException(argument.Name, context.Location);
                }

                value = argument.Default;
            }
            else
            {
                var evaluableExpression = context.Arguments[i];
                value = evaluableExpression.Evaluate(programContext);
            }

            if (!argument.Type.Is(value))
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(argument.Type.ToString(), value.ToString(),
                    context.Location);
            }

            //We need add argument with function to functions repository
            if (value is FunctionValue functionValue)
            {
                var functionInstance = functionValue.Value.Copy(argument.Name, Body);

                programContext.Functions.Add(functionInstance, context.Location, false);
            }

            programContext.Variables.Add(new UserVariableInstance
            {
                Name = argument.Name,
                Module = Module,
                Root = Body,
                Value = value
            }, context.Location, false);
        }

        Body.Token = context.Token;

        var result = Body.Evaluate(programContext);
        context.ProgramContext.Clear(Body);

        if (!Return.Is(result))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name,
                Return.AsString(programContext, context.Location), result.Type.ToString(), context.Location);
        }

        return result;
    }

    public bool TryChange(ProgramContext programContext, IInstance instance, Location location,
        out AbstractLanguageException exception)
    {
        if (instance is not UserFunctionInstance functionInstance)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        if (!Return.Is(functionInstance.Return))
        {
            exception = new IncorrectTypeException(Return.ToString(), functionInstance.Return.ToString(), location);
            return false;
        }

        Body = functionInstance.Body;

        exception = null;
        return false;
    }

    public FunctionValue Create()
    {
        return new FunctionValue(this);
    }

    public IFunctionInstance Copy(string name = null, FunctionBodyExpression root = null)
    {
        return new UserFunctionInstance
        {
            Name = name ?? Name,
            Module = Module,
            Arguments = Arguments,
            Body = Body,
            IsAsync = IsAsync,
            Return = Return,
            Access = Access,
            Root = root ?? Root
        };
    }
}