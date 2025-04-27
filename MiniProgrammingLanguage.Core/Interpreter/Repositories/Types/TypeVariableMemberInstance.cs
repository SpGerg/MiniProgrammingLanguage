using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeVariableMemberInstance : ITypeMember
{
    public required string Parent { get; init; }
    
    public required string Module { get; init; }
    
    public required ITypeMemberIdentification Identification { get; init; }

    public required AbstractValue Default { get; init; } = new NoneValue();
    
    public IEnumerable<string> Attributes { get; init; } = Array.Empty<string>();

    public AccessType Access { get; init; }

    public bool IsFunctionInstance { get; init; }
    
    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;
}