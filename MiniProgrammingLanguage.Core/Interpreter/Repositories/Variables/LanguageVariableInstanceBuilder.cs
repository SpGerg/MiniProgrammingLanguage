using System;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class LanguageVariableInstanceBuilder
{
    public string Name { get; set; }

    public string Module { get; set; } = "global";

    public Func<VariableGetterContext, AbstractValue> Bind { get; set; }
    
    public ObjectTypeValue Type { get; set; } = ObjectTypeValue.Any;
    
    public FunctionBodyExpression Root { get; set; }
    
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

    public LanguageVariableInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        Type = typeValue;
        
        return this;
    }
    
    public LanguageVariableInstanceBuilder SetBind(Func<VariableGetterContext, AbstractValue> bind)
    {
        Bind = bind;
        
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
            Bind = Bind,
            Type = Type,
            Root = Root
        };
    }
}