using System;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.SharpKit.Functions;

namespace MiniProgrammingLanguage.SharpKit;

public static class SharpKitModule
{
    private const string Name = "sharp-kit";
    
    public static ImplementModule Create()
    {
        var requireDependency = RequireDependencyFunction.Create();
        
        return new ImplementModule
        {
            Name = Name,
            Types = Array.Empty<ITypeInstance>(),
            Functions = new[] { requireDependency },
            Variables = Array.Empty<IVariableInstance>(),
            Enums = Array.Empty<IEnumInstance>()
        };
    }
}