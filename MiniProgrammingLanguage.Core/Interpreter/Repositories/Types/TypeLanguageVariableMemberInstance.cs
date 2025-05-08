using System;
using System.Collections.Generic;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeLanguageVariableMemberInstance : ITypeLanguageVariableMember
{
    public required string Parent { get; init; }

    public required string Module { get; init; }

    public required ITypeMemberIdentification Identification { get; init; }

    public AbstractValue Default { get; init; } = NoneValue.Instance;

    public IEnumerable<string> Attributes { get; init; } = Array.Empty<string>();

    public AccessType Access { get; init; }

    public ObjectTypeValue Type { get; set; } = ObjectTypeValue.Any;

    public Func<TypeMemberGetterContext, AbstractValue> GetBind { get; set; }

    public Action<TypeMemberSetterContext> SetBind { get; set; }

    public PropertyInfo Property { get; set; }

    public bool IsFunctionInstance { get; init; }
}