using System;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionInstanceBuilder
{
    public string Name { get; set; }
    
    public string Module { get; set; }

    public FunctionArgument[] Arguments { get; set; } = Array.Empty<FunctionArgument>();

    public Func<FunctionExecuteContext, AbstractValue> Bind { get; set; }
    
    public ObjectTypeValue Return { get; set; } = ObjectTypeValue.Any;
    
    public FunctionBodyExpression Root { get; set; }
    
    public bool IsAsync { get; set; }
    
    public LanguageFunctionInstanceBuilder SetName(string name)
    {
        Name = name;
        
        return this;
    }
    
    public LanguageFunctionInstanceBuilder SetModule(string module)
    {
        Module = module;
        
        return this;
    }

    public LanguageFunctionInstanceBuilder SetArguments(params FunctionArgument[] arguments)
    {
        Arguments = arguments;
        
        return this;
    }
    
    public LanguageFunctionInstanceBuilder SetAsync(bool isAsync)
    {
        IsAsync = isAsync;
        
        return this;
    }

    public LanguageFunctionInstanceBuilder SetReturn(ObjectTypeValue returnType)
    {
        Return = returnType;
        
        return this;
    }
    
    public LanguageFunctionInstanceBuilder SetBind(Func<FunctionExecuteContext, AbstractValue> bind)
    {
        Bind = bind;
        
        return this;
    }
    
    public LanguageFunctionInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        Root = root;
        
        return this;
    }

    public LanguageFunctionInstance Build()
    {
        return new LanguageFunctionInstance
        {
            Name = Name,
            Module = Module,
            Arguments = Arguments,
            IsAsync = IsAsync,
            Bind = Bind,
            Return = Return,
            Root = Root
        };
    }
}