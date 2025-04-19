using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter;

public class ProgramContext
{
    public ProgramContext(IEnumerable<ITypeInstance> types = null, IEnumerable<IFunctionInstance> functions = null, IEnumerable<IVariableInstance> variables = null)
    {
        if (variables is not null)
        {
            foreach (var variable in variables)
            {
                Variables.Add(variable);
            }
        }

        if (functions is not null)
        {
            foreach (var function in functions)
            {
                Functions.Add(function);
            }
        }

        if (types is not null)
        {
            foreach (var type in types)
            {
                Types.Add(type);
            }
        }
    }

    public string Module { get; set; } = "global";
    
    public IVariablesRepository Variables { get; } = new VariablesRepository();

    public ITypesRepository Types { get; } = new TypesRepository();

    public IFunctionsRepository Functions { get; } = new FunctionsRepository();

    public ITasksRepository Tasks { get; } = new TasksRepository();
}