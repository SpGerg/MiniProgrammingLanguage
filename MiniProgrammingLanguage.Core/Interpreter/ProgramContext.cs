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
    public ProgramContext(string filepath, params ImplementModule[] modules)
    {
        Filepath = filepath;

        foreach (var module in modules)
        {
            Import(module);
        }
    }

    public string Module { get; set; } = "global";
    
    public string Filepath { get; set; }

    public bool IsGlobal => Module is "global";

    public IEnumerable<string> Imported => _importedModules;

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
        Enums.AddRange(programContext.Enums.Entities);
        Tasks.AddRange(programContext.Tasks.Entities);
    }
    
    public void Import(ImplementModule implementModule)
    {
        if (!implementModule.IsGlobal && _importedModules.Contains(implementModule.Name))
        {
            InterpreterThrowHelper.ThrowCyclicImportException(implementModule.Name, implementModule.Location);
        }
        
        _importedModules.Push(implementModule.Name);
        
        Types.AddRange(implementModule.Types);
        Functions.AddRange(implementModule.Functions);
        Enums.AddRange(implementModule.Enums);
        Variables.AddRange(implementModule.Variables);
    }

    public void Clear(FunctionBodyExpression functionBodyExpression)
    {
        Types.Clear(functionBodyExpression);
        Functions.Clear(functionBodyExpression);
        Variables.Clear(functionBodyExpression);
    }
}