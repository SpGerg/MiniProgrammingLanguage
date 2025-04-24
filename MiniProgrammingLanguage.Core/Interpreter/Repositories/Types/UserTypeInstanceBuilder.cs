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
        Name = name;

        return this;
    }
    
    public UserTypeInstanceBuilder SetMembers(params ITypeMember[] members)
    {
        Members = members;
        
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
            Members = Members,
            Root = Root
        };
    }
}