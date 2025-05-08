using System;
using System.Collections.Generic;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeLanguageVariableMemberInstanceBuilder
{
    public string Parent { get; set; }

    public string Module { get; set; }

    public ITypeMemberIdentification Identification { get; set; }

    public IEnumerable<string> Attributes { get; set; } = Array.Empty<string>();

    public AccessType Access { get; set; }

    public ObjectTypeValue Type { get; set; } = ObjectTypeValue.Any;

    public Func<TypeMemberGetterContext, AbstractValue> GetBind { get; set; }

    public Action<TypeMemberSetterContext> SetBind { get; set; }

    public TypeLanguageVariableMemberInstanceBuilder SetParent(string parent)
    {
        Parent = parent;

        return this;
    }
    
    public TypeLanguageVariableMemberInstanceBuilder SetModule(string module)
    {
        Module = module;

        return this;
    }
    
    public TypeLanguageVariableMemberInstanceBuilder SetAttributes(IEnumerable<string> attributes)
    {
        Attributes = attributes;

        return this;
    }

    public TypeLanguageVariableMemberInstanceBuilder SetType(ObjectTypeValue typeValue)
    {
        Type = typeValue;

        return this;
    }

    public TypeLanguageVariableMemberInstanceBuilder SetName(string name)
    {
        Identification = new KeyTypeMemberIdentification
        {
            Identifier = name
        };

        return this;
    }

    public TypeLanguageVariableMemberInstanceBuilder SetAccess(AccessType accessType)
    {
        Access = accessType;

        return this;
    }
    
    public TypeLanguageVariableMemberInstanceBuilder SetGetBind(Func<TypeMemberGetterContext, AbstractValue> bind)
    {
        GetBind = bind;

        return this;
    }

    
    public TypeLanguageVariableMemberInstanceBuilder SetSetBind(Action<TypeMemberSetterContext> bind)
    {
        SetBind = bind;

        return this;
    }

    public TypeLanguageVariableMemberInstance Build()
    {
        return new TypeLanguageVariableMemberInstance
        {
            Parent = Parent,
            Module = Module,
            Type = Type,
            GetBind = GetBind,
            SetBind = SetBind,
            Identification = Identification,
            Attributes = Attributes,
            Access = Access
        };
    }
}