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
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = Type,
            Identification = Identification,
            Default = Default,
            IsReadOnly = IsReadOnly,
            Root = Root
        };
    }
    
    public TypeVariableMemberInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = typeValue,
            Identification = Identification,
            Default = Default,
            IsReadOnly = IsReadOnly,
            Root = Root
        };
    }
    
    public TypeVariableMemberInstanceBuilder SetIdentification(KeyTypeMemberIdentification identification)
    {
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = Type,
            Identification = identification,
            Default = Default,
            IsReadOnly = IsReadOnly,
            Root = Root
        };
    }
    
    public TypeVariableMemberInstanceBuilder SetDefault(AbstractValue defaultValue)
    {
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = Type,
            Identification = Identification,
            Default = defaultValue,
            IsReadOnly = IsReadOnly,
            Root = Root
        };
    }
    
    public TypeVariableMemberInstanceBuilder SetReadOnly(bool isReadOnly)
    {
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = Type,
            Identification = Identification,
            Default = Default,
            IsReadOnly = isReadOnly,
            Root = Root
        };
    }

    public TypeVariableMemberInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        return new TypeVariableMemberInstanceBuilder
        {
            Parent = Parent,
            Type = Type,
            Identification = Identification,
            Default = Default,
            IsReadOnly = IsReadOnly,
            Root = root
        };
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