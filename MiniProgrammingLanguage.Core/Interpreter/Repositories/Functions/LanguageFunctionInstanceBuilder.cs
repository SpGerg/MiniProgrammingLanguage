using System;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;

public class LanguageFunctionInstanceBuilder
{
    public string Name { get; set; }

    public FunctionArgument[] Arguments { get; init; } = Array.Empty<FunctionArgument>();

    public Func<FunctionExecuteContext, AbstractValue> Bind { get; set; }
    
    public ObjectTypeValue Return { get; set; } = ObjectTypeValue.Any;
    
    public FunctionBodyExpression Root { get; set; }
    
    public bool IsAsync { get; set; }
    
    public LanguageFunctionInstanceBuilder SetName(string name)
    {
        return new LanguageFunctionInstanceBuilder()
        {
            Name = name,
            Bind = Bind,
            Return = Return,
            Arguments = Arguments,
            Root = Root
        };
    }
    
    public LanguageFunctionInstanceBuilder SetArguments(params FunctionArgument[] arguments)
    {
        return new LanguageFunctionInstanceBuilder()
        {
            Name = Name,
            Bind = Bind,
            Return = Return,
            Arguments = arguments,
            Root = Root
        };
    }
    
    public LanguageFunctionInstanceBuilder SetAsync(bool isAsync)
    {
        return new LanguageFunctionInstanceBuilder()
        {
            Name = Name,
            IsAsync = isAsync,
            Bind = Bind,
            Return = Return,
            Arguments = Arguments,
            Root = Root
        };
    }

    public LanguageFunctionInstanceBuilder SetReturn(ObjectTypeValue returnType)
    {
        return new LanguageFunctionInstanceBuilder
        {
            Name = Name,
            IsAsync = IsAsync,
            Bind = Bind,
            Return = returnType,
            Arguments = Arguments,
            Root = Root
        };
    }
    
    public LanguageFunctionInstanceBuilder SetBind(Func<FunctionExecuteContext, AbstractValue> bind)
    {
        return new LanguageFunctionInstanceBuilder
        {
            Name = Name,
            IsAsync = IsAsync,
            Bind = bind,
            Return = Return,
            Arguments = Arguments,
            Root = Root
        };
    }
    
    public LanguageFunctionInstanceBuilder SetRoot(FunctionBodyExpression root)
    {
        return new LanguageFunctionInstanceBuilder
        {
            Name = Name,
            IsAsync = IsAsync,
            Bind = Bind,
            Return = Return,
            Arguments = Arguments,
            Root = Root
        };
    }

    public LanguageFunctionInstance Build()
    {
        return new LanguageFunctionInstance
        {
            Name = Name,
            Arguments = Arguments,
            IsAsync = IsAsync,
            Bind = Bind,
            Return = Return,
            Root = Root
        };
    }
}