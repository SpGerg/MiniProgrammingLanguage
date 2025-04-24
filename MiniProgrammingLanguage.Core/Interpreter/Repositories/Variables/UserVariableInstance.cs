using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class UserVariableInstance : IVariableInstance
{
    public required string Name { get; init; }
    
    public required FunctionBodyExpression Root { get; init; }
    
    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;

    public AbstractValue Value { get; set; } = new NoneValue();
    
    public AbstractValue GetValue(VariableGetterContext context)
    {
        if (!Type.Is(Value))
        {
            InterpreterThrowHelper.ThrowInvalidReturnTypeException(Name, Type.AsString(context.ProgramContext, context.Location), Value.Type.ToString(), context.Location);
        }

        return Value;
    }
    
    public bool TryChange(ProgramContext programContext, IRepositoryInstance repositoryInstance, Location location, out AbstractLanguageException exception)
    {
        if (repositoryInstance is not IVariableInstance variableInstance)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        if (!Type.Is(variableInstance.Type))
        {
            exception = new IncorrectTypeException(Type.ToString(), variableInstance.Type.ToString(), location);
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