using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.SharpKit.Functions;
using MiniProgrammingLanguage.SharpKit.Variables;

namespace MiniProgrammingLanguage.SharpKit;

public static class SharpKitModule
{
    /// <summary>
    /// Name
    /// </summary>
    public const string Name = "sharp-kit";
    
    /// <summary>
    /// Create sharp kit implement module
    /// </summary>
    /// <returns></returns>
    public static ImplementModule Create()
    {
        var createFromExtender = CreateFromExtenderFunction.Create();
        var requireDependency = RequireDependencyFunction.Create();
        var createBasedOn = BindFunction.Create();
        var createType = GetTypeFunction.Create();
        var getBase = GetBaseFunction.Create();

        var sharpKitVariable = SharpKitVariable.Create();
        
        return new ImplementModule
        {
            Name = Name,
            Types = Array.Empty<ITypeInstance>(),
            Functions = new[] { createFromExtender, requireDependency, createBasedOn, createType, getBase },
            Variables = new [] { sharpKitVariable },
            Enums = Array.Empty<IEnumInstance>()
        };
    }
}