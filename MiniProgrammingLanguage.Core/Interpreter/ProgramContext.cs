using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter;

public class ProgramContext
{
    public ProgramContext(string filepath, IEnumerable<ITypeInstance> types = null, IEnumerable<IFunctionInstance> functions = null, IEnumerable<IEnumInstance> enums = null, IEnumerable<IVariableInstance> variables = null)
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
        
        if (enums is not null)
        {
            foreach (var enumInstance in enums)
            {
                Enums.Add(enumInstance);
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

    public bool IsGlobal => Module is "global";

    public IReadOnlyCollection<string> Imported => _importedModules;

    public ITypesRepository Types { get; } = new TypesRepository();
    
    public IFunctionsRepository Functions { get; } = new FunctionsRepository();

    public IEnumsRepository Enums { get; } = new EnumsRepository();
    
    public IVariablesRepository Variables { get; } = new VariablesRepository();

    public ITasksRepository Tasks { get; } = new TasksRepository();

    private readonly Stack<string> _importedModules = new();

    public void Import(ProgramContext programContext, Location location)
    {
        if (!programContext.IsGlobal && _importedModules.Contains(programContext.Module))
        {
            InterpreterThrowHelper.ThrowCyclicImportException(programContext.Module, location);
        }
        
        _importedModules.Push(programContext.Module);
        
        Types.AddRange(programContext.Types.Entities);
        Functions.AddRange(programContext.Functions.Entities);
        Variables.AddRange(programContext.Variables.Entities);
        Tasks.AddRange(programContext.Tasks.Entities);
    }

    public void Clear(FunctionBodyExpression functionBodyExpression)
    {
        Types.Clear(functionBodyExpression);
        Functions.Clear(functionBodyExpression);
        Variables.Clear(functionBodyExpression);
    }
}