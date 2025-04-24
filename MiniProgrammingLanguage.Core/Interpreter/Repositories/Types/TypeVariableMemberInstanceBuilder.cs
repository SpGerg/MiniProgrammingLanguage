using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeVariableMemberInstanceBuilder
{
    public string Parent { get; set; }
    
    public KeyTypeMemberIdentification Identification { get; set; }
    
    public ObjectTypeValue Type { get; set; }
    
    public AbstractValue Default { get; set; }
    
    public bool IsReadOnly { get; set; }

    public FunctionBodyExpression Root { get; set; }
    
    public TypeVariableMemberInstanceBuilder SetParent(string parent)
    {
        Parent = parent;
        
        return this;
    }
    
    public TypeVariableMemberInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        Type = typeValue;
        
        return this;
    }
    
    public TypeVariableMemberInstanceBuilder SetIdentification(KeyTypeMemberIdentification identification)
    {
        Identification = identification;
        
        return this;
    }
    
    public TypeVariableMemberInstanceBuilder SetDefault(AbstractValue defaultValue)
    {
        Default = defaultValue;
        
        return this;
    }
    
    public TypeVariableMemberInstanceBuilder SetReadOnly(bool isReadOnly)
    {
        IsReadOnly = isReadOnly;
        
        return this;
    }

    public TypeVariableMemberInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        Root = root;

        return this;
    }

    public TypeVariableMemberInstance Build()
    {
        return new TypeVariableMemberInstance
        {
            Parent = Parent,
            Default = Default,
            Identification = Identification,
            IsReadonly = IsReadOnly
        };
    }
}