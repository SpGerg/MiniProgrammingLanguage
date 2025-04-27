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
    public const string Name = "sharp-kit";
    
    public static ImplementModule Create()
    {
        var requireDependency = RequireDependencyFunction.Create();
        var createBasedOn = CreateBasedOnFunction.Create();
        var createType = CreateTypeFunction.Create();
        var getBase = GetBaseFunction.Create();

        var sharpKitVariable = SharpKitVariable.Create();
        
        return new ImplementModule
        {
            Name = Name,
            Types = Array.Empty<ITypeInstance>(),
            Functions = new[] { requireDependency, createBasedOn, createType, getBase },
            Variables = new [] { sharpKitVariable },
            Enums = Array.Empty<IEnumInstance>()
        };
    }
}