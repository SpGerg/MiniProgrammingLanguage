using System;
using System.Collections.Generic;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeLanguageFunctionMemberInstanceBuilder
{
    public string Parent { get; set; }

    public string Module { get; set; }

    public FunctionTypeMemberIdentification Identification { get; set; }
    
    public FunctionArgument[] Arguments { get; set; }

    public ObjectTypeValue Return { get; set; }
    
    public Func<TypeFunctionExecuteContext, AbstractValue> Bind { get; set; }
    
    public AccessType Access { get; set; }

    public bool IsAsync { get; set; }

    public IEnumerable<string> Attributes { get; set; } = Array.Empty<string>();

    public TypeLanguageFunctionMemberInstanceBuilder SetParent(string parent)
    {
        Parent = parent;

        return this;
    }

    public TypeLanguageFunctionMemberInstanceBuilder SetModule(string module)
    {
        Module = module;

        return this;
    }
    
    public TypeLanguageFunctionMemberInstanceBuilder SetName(string name)
    {
        Identification = new FunctionTypeMemberIdentification
        {
            Identifier = name
        };

        return this;
    }

    public TypeLanguageFunctionMemberInstanceBuilder SetReturn(ObjectTypeValue typeValue)
    {
        Return = typeValue;

        return this;
    }
    
    public TypeLanguageFunctionMemberInstanceBuilder SetArguments(params FunctionArgument[] functionArguments)
    {
        Arguments = functionArguments;

        return this;
    }
    
    public TypeLanguageFunctionMemberInstanceBuilder SetBind(Func<TypeFunctionExecuteContext, AbstractValue> bind)
    {
        Bind = bind;

        return this;
    }

    public TypeLanguageFunctionMemberInstanceBuilder SetAccess(AccessType accessType)
    {
        Access = accessType;

        return this;
    }
    
    public TypeLanguageFunctionMemberInstanceBuilder SetAsync(bool async)
    {
        IsAsync = async;

        return this;
    }
    
    public TypeLanguageFunctionMemberInstanceBuilder SetAttributes(IEnumerable<string> attributes)
    {
        Attributes = attributes;

        return this;
    }

    public TypeLanguageFunctionMemberInstance Build()
    {
        return new TypeLanguageFunctionMemberInstance
        {
            Parent = Parent,
            Module = Module,
            Bind = Bind,
            Return = Return,
            IsAsync = IsAsync,
            Arguments = Arguments,
            Identification = Identification,
            Access = Access
        };
    }
}