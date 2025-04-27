using System;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class LanguageVariableInstanceBuilder
{
    public string Name { get; set; }

    public string Module { get; set; } = "global";

    public Func<VariableGetterContext, AbstractValue> GetBind { get; set; }
    
    public Func<VariableSetterContext, AbstractValue> SetBind { get; set; }
    
    public ObjectTypeValue Type { get; set; } = ObjectTypeValue.Any;
    
    public FunctionBodyExpression Root { get; set; }
    
    public AccessType Access { get; set; }
    
    public LanguageVariableInstanceBuilder SetName(string name)
    {
        Name = name;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetModule(string module)
    {
        Module = module;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetAccess(AccessType accessType)
    {
        Access = accessType;
        
        return this;
    }

    public LanguageVariableInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        Type = typeValue;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetGetBindFunc(Func<VariableGetterContext, AbstractValue> bind)
    {
        GetBind = bind;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetSetBindFunc(Func<VariableSetterContext, AbstractValue> bind)
    {
        SetBind = bind;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        Root = root;
        
        return this;
    }

    public LanguageVariableInstance Build()
    {
        return new LanguageVariableInstance
        {
            Name = Name,
            Module = Module,
            GetBind = GetBind,
            SetBind = SetBind,
            Access = Access,
            Type = Type,
            Root = Root
        };
    }
}