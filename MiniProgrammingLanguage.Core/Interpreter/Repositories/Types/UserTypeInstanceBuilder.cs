using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class UserTypeInstanceBuilder
{
    public string Name { get; set; }

    public string Module { get; set; }

    public List<ITypeMember> Members { get; set; } = new();
    
    public AccessType Access { get; set; }

    public FunctionBodyExpression Root { get; set; }

    public UserTypeInstanceBuilder SetName(string name)
    {
        Name = name;

        return this;
    }

    public UserTypeInstanceBuilder SetModule(string module)
    {
        Module = module;

        return this;
    }

    public UserTypeInstanceBuilder SetMembers(params ITypeMember[] members)
    {
        Members = members.ToList();

        return this;
    }
    
    public UserTypeInstanceBuilder AddMember(ITypeMember member)
    {
        Members.Add(member);

        return this;
    }
    
    public UserTypeInstanceBuilder SetAccess(AccessType access)
    {
        Access = access;

        return this;
    }

    public UserTypeInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        Root = root;

        return this;
    }

    public UserTypeInstance Build()
    {
        return new UserTypeInstance
        {
            Name = Name,
            Module = Module,
            Members = Members,
            Access = Access,
            Root = Root
        };
    }
}