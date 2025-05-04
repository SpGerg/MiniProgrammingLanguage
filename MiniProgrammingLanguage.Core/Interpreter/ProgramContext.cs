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
    /// <summary>
    /// Create instance of program context
    /// </summary>
    /// <param name="filepath">Filepath</param>
    /// <param name="modules">Default modules</param>
    public ProgramContext(string filepath, params ImplementModule[] modules)
    {
        Filepath = filepath;

        foreach (var module in modules)
        {
            Import(module);
        }
    }

    /// <summary>
    /// Module.
    /// By default - 'global'
    /// </summary>
    public string Module { get; set; } = "global";

    /// <summary>
    /// Script filepath.
    /// </summary>
    public string Filepath { get; set; }

    /// <summary>
    /// Is module name 'global'.
    /// </summary>
    public bool IsGlobal => Module is "global";

    /// <summary>
    /// Imported modules.
    /// </summary>
    public IEnumerable<string> Imported => _importedModules;

    /// <summary>
    /// Types
    /// </summary>
    public ITypesRepository Types { get; } = new TypesRepository();

    /// <summary>
    /// Functions
    /// </summary>
    public IFunctionsRepository Functions { get; } = new FunctionsRepository();

    /// <summary>
    /// Enums
    /// </summary>
    public IEnumsRepository Enums { get; } = new EnumsRepository();

    /// <summary>
    /// Variables
    /// </summary>
    public IVariablesRepository Variables { get; } = new VariablesRepository();

    /// <summary>
    /// Tasks
    /// </summary>
    public ITasksRepository Tasks { get; } = new TasksRepository();

    private readonly Stack<string> _importedModules = new();

    /// <summary>
    /// Import program context.
    /// </summary>
    /// <param name="programContext">Can be null</param>
    /// <param name="location">Location</param>
    public void Import(ProgramContext programContext, Location location)
    {
        if (programContext is null)
        {
            return;
        }

        if (_importedModules.Contains(programContext.Module))
        {
            return;
        }

        _importedModules.Push(programContext.Module);

        Types.AddRange(programContext.Types.Entities, false);
        Functions.AddRange(programContext.Functions.Entities, false);
        Variables.AddRange(programContext.Variables.Entities, false);
        Enums.AddRange(programContext.Enums.Entities, false);
        Tasks.AddRange(programContext.Tasks.Entities);
    }

    /// <summary>
    /// Import module
    /// </summary>
    /// <param name="implementModule">Can be null</param>
    public void Import(ImplementModule implementModule)
    {
        if (implementModule is null)
        {
            return;
        }

        if (!implementModule.IsGlobal && _importedModules.Contains(implementModule.Name))
        {
            return;
        }

        _importedModules.Push(implementModule.Name);

        Types.AddRange(implementModule.Types);
        Functions.AddRange(implementModule.Functions);
        Enums.AddRange(implementModule.Enums);
        Variables.AddRange(implementModule.Variables);
    }

    /// <summary>
    /// Clear types, functions, variables, enums in given body.
    /// </summary>
    /// <param name="functionBodyExpression"></param>
    public void Clear(FunctionBodyExpression functionBodyExpression)
    {
        Types.Clear(functionBodyExpression);
        Functions.Clear(functionBodyExpression);
        Variables.Clear(functionBodyExpression);
        Enums.Clear(functionBodyExpression);
    }
}