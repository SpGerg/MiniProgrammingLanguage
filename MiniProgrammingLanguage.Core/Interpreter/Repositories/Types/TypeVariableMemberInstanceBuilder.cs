using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeVariableMemberInstanceBuilder
{
    public string Parent { get; set; }
    
    public string Module { get; set; }
    
    public KeyTypeMemberIdentification Identification { get; set; }
    
    public ObjectTypeValue Type { get; set; }
    
    public AbstractValue Default { get; set; }
    
    public AccessType Access { get; set; }

    public FunctionBodyExpression Root { get; set; }
    
    public TypeVariableMemberInstanceBuilder SetParent(string parent)
    {
        Parent = parent;
        
        return this;
    }
    
    public TypeVariableMemberInstanceBuilder SetModule(string module)
    {
        Module = module;
        
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
    
    public TypeVariableMemberInstanceBuilder SetAccess(AccessType accessType)
    {
        Access = accessType;
        
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
            Module = Module,
            Default = Default,
            Identification = Identification,
            Access = Access
        };
    }
}