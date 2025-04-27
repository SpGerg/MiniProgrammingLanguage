using System;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeLanguageFunctionMember : ITypeLanguageMember
{
    Func<TypeFunctionExecuteContext, AbstractValue> Bind { get; set; }

    MethodInfo Method { get; set; }
}