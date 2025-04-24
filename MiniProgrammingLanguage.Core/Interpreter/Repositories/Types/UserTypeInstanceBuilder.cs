using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class UserTypeInstanceBuilder
{
    public string Name { get; set; }
    
    public IReadOnlyList<ITypeMember> Members { get; set; }

    public FunctionBodyExpression Root { get; set; }
    
    public UserTypeInstanceBuilder SetName(string name)
    {
        return new UserTypeInstanceBuilder
        {
            Name = name,
            Members = Members,
            Root = Root
        };
    }
    
    public UserTypeInstanceBuilder SetMembers(params ITypeMember[] members)
    {
        return new UserTypeInstanceBuilder
        {
            Name = Name,
            Members = members,
            Root = Root
        };
    }

    public UserTypeInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        return new UserTypeInstanceBuilder
        {
            Name = Name,
            Members = Members,
            Root = Root
        };
    }

    public UserTypeInstance Build()
    {
        return new UserTypeInstance
        {
            Name = Name,
            Members = Members,
            Root = Root
        };
    }
}