using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums;

public sealed class UserEnumInstanceBuilder
{
    public string Name { get; set; }

    public string Module { get; set; } = "global";

    public Dictionary<string, int> Members { get; set; } = new();

    public FunctionBodyExpression Root { get; set; }

    public AccessType Access { get; set; }

    public UserEnumInstanceBuilder SetName(string name)
    {
        Name = name;

        return this;
    }

    public UserEnumInstanceBuilder SetModule(string module)
    {
        Module = module;

        return this;
    }

    public UserEnumInstanceBuilder SetAccess(AccessType accessType)
    {
        Access = accessType;

        return this;
    }
    
    public UserEnumInstanceBuilder AddMember(string name, int value)
    {
        Members.Add(name, value);

        return this;
    }


    public UserEnumInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        Root = root;

        return this;
    }

    public UserEnumInstance Build()
    {
        return new UserEnumInstance
        {
            Name = Name,
            Module = Module,
            Members = Members,
            Access = Access,
            Root = Root
        };
    }
}