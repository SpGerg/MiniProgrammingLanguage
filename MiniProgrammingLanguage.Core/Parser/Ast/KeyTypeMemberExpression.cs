using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class KeyTypeMemberExpression : AbstractExpression, ITypeMemberExpression
{
    public KeyTypeMemberExpression(string name, string parent, ObjectTypeValue type, IEnumerable<string> attributes,
        AccessType access, Location location) : base(location)
    {
        Parent = parent;
        Name = name;
        Type = type;
        Attributes = attributes;
        Access = access;
    }

    public string Parent { get; }

    public string Name { get; }

    public ObjectTypeValue Type { get; }

    public IEnumerable<string> Attributes { get; }

    public AccessType Access { get; }

    public ITypeMember Create(string module)
    {
        if (Access.HasFlag(AccessType.Bindable))
        {
            return new TypeLanguageVariableMemberInstance()
            {
                Parent = Parent,
                Module = module,
                Identification = new KeyTypeMemberIdentification
                {
                    Identifier = Name
                },
                Type = Type,
                Default = NoneValue.Instance,
                Access = Access,
                Attributes = Attributes
            };
        }

        return new TypeVariableMemberInstance
        {
            Parent = Parent,
            Module = module,
            Identification = new KeyTypeMemberIdentification
            {
                Identifier = Name
            },
            Type = Type,
            Default = NoneValue.Instance,
            Access = Access,
            Attributes = Attributes
        };
    }
}