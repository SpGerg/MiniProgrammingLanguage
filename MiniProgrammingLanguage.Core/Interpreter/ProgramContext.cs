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
    public ProgramContext(string filepath, IEnumerable<ITypeInstance> types = null, IEnumerable<IFunctionInstance> functions = null, IEnumerable<IVariableInstance> variables = null)
    {
        Filepath = filepath;
        
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
    
    public string Filepath { get; set; }
    
    public ITypesRepository Types { get; } = new TypesRepository();
    
    public IFunctionsRepository Functions { get; } = new FunctionsRepository();
    
    public IVariablesRepository Variables { get; } = new VariablesRepository();

    public ITasksRepository Tasks { get; } = new TasksRepository();

    public void Import(ProgramContext programContext)
    {
        Types.AddRange(programContext.Types.Entities);
        Functions.AddRange(programContext.Functions.Entities);
        Variables.AddRange(programContext.Variables.Entities);
        Tasks.AddRange(programContext.Tasks.Entities);
    }
}