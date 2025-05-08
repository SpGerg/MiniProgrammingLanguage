using System;
using System.Collections.Generic;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeLanguageFunctionMemberInstance : ITypeLanguageFunctionMember
{
    public required string Parent { get; init; }

    public required string Module { get; init; }

    public required ITypeMemberIdentification Identification { get; init; }

    public required ObjectTypeValue Return { get; init; }

    public required bool IsAsync { get; init; }

    public required FunctionArgument[] Arguments { get; init; }

    public Func<TypeFunctionExecuteContext, AbstractValue> Bind { get; set; }

    public MethodInfo Method { get; set; }

    public AccessType Access { get; init; }

    public IEnumerable<string> Attributes { get; init; } = Array.Empty<string>();

    public ObjectTypeValue Type { get; } = ObjectTypeValue.Function;

    public AbstractValue Default => NoneValue.Instance;

    public FunctionValue Create(FunctionBodyExpression root = null)
    {
        return new FunctionValue(new LanguageFunctionInstance
        {
            Name = Identification.Identifier,
            Module = Module,
            Bind = context => Bind.Invoke(new TypeFunctionExecuteContext
            {
                ProgramContext = context.ProgramContext,
                Root = context.Root,
                Arguments = context.ArgumentsExpressions,
                Location = context.Location,
                Member = null,
                TokenSource = context.TokenSource,
                Type = null
            }),
            Root = root,
            Arguments = Arguments,
            IsAsync = IsAsync,
            Access = Access,
            Return = Return
        });
    }
}