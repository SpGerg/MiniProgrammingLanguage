using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class NoneValue : AbstractValue
{
    public static NoneValue Instance { get; } = new();
    
    public NoneValue() : base(string.Empty)
    {
    }

    public override ValueType Type => ValueType.None;

    public override ValueType[] CanCast { get; } = { };

    public override bool IsValueType => false;

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return Instance;
    }
}