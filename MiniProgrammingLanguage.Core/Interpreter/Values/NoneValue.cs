using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class NoneValue : AbstractValue
{
    public override ValueType Type => ValueType.None;

    public override ValueType[] CanCast { get; } = {};
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }
}