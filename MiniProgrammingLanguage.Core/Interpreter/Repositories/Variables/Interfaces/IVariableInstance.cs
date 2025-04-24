using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;

public interface IVariableInstance : IInstance
{
    ObjectTypeValue Type { get; }

    AbstractValue GetValue(VariableGetterContext variableGetterContext);
}