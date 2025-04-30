using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeFunctionMemberInstance : ITypeMember
{
    public required string Parent { get; init; }

    public required string Module { get; init; }

    public required ITypeMemberIdentification Identification { get; init; }

    public required ObjectTypeValue Return { get; init; }

    public required bool IsAsync { get; init; }

    public required FunctionArgument[] Arguments { get; init; }

    public AccessType Access { get; init; }

    public IEnumerable<string> Attributes { get; init; } = Array.Empty<string>();

    public ObjectTypeValue Type { get; } = ObjectTypeValue.Function;

    public IFunctionInstance Value { get; set; }

    public AbstractValue Default { get; } = new NoneValue();

    public FunctionValue Create(FunctionBodyExpression root = null)
    {
        return new FunctionValue(new UserFunctionInstance
        {
            Name = Identification.Identifier,
            Module = Module,
            Root = root,
            Body = null,
            Arguments = Arguments,
            IsAsync = IsAsync,
            Access = Access,
            Return = Return
        });
    }
}