using System;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class LanguageVariableInstanceBuilder
{
    public string Name { get; set; }

    public Func<VariableGetterContext, AbstractValue> Bind { get; set; }
    
    public ObjectTypeValue Type { get; set; } = ObjectTypeValue.Any;
    
    public FunctionBodyExpression Root { get; set; }
    
    public LanguageVariableInstanceBuilder SetName(string name)
    {
        return new LanguageVariableInstanceBuilder
        {
            Name = name,
            Bind = Bind,
            Type = Type,
            Root = Root
        };
    }

    public LanguageVariableInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        return new LanguageVariableInstanceBuilder
        {
            Name = Name,
            Bind = Bind,
            Type = typeValue,
            Root = Root
        };
    }
    
    public LanguageVariableInstanceBuilder SetBind(Func<VariableGetterContext, AbstractValue> bind)
    {
        return new LanguageVariableInstanceBuilder
        {
            Name = Name,
            Bind = bind,
            Type = Type,
            Root = Root
        };
    }
    
    public LanguageVariableInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        return new LanguageVariableInstanceBuilder
        {
            Name = Name,
            Bind = Bind,
            Type = Type,
            Root = Root
        };
    }

    public LanguageVariableInstance Build()
    {
        return new LanguageVariableInstance
        {
            Name = Name,
            Bind = Bind,
            Type = Type,
            Root = Root
        };
    }
}