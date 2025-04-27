using System;
using System.Reflection;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeLanguageMember : ITypeMember
{
    Func<TypeMemberGetterContext, AbstractValue> GetBind { get; set; }
    
    Action<TypeMemberSetterContext> SetBind { get; set; }
    
    PropertyInfo Property { get; set; }
}