using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class UserVariableInstance : IVariableInstance
{
    public required string Name { get; init; }
    
    public required string Module { get; init; }

    public required FunctionBodyExpression Root { get; init; }

    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;

    public AbstractValue Value { get; set; } = new NoneValue();
    
    public AccessType Access { get; init; }

    public AbstractValue GetValue(VariableGetterContext context)
    {
        if (!Type.Is(Value))
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(Type.ToString(), Value.ToString(), context.Location);
        }

        return Value.IsValueType ? Value.Copy() : Value;
    }
    
    public bool TryChange(ProgramContext programContext, IInstance instance, Location location, out AbstractLanguageException exception)
    {
        if (instance is not IVariableInstance variableInstance)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        if (!Type.Is(variableInstance.Type))
        {
            exception = new IncorrectTypeException(Type.ToString(), variableInstance.Type.ToString(), location);
            return false;
        }

        if (Access.HasFlag(AccessType.ReadOnly) && programContext.Module != Module)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        Value = variableInstance.GetValue(new VariableGetterContext
        {
            ProgramContext = programContext,
            Location = location,
        });

        exception = null;
        return true;
    }
}