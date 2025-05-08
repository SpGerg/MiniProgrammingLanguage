using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class VoidValue : AbstractValue
{
    public static VoidValue Instance { get; } = new();
    
    public VoidValue() : base(string.Empty)
    {
    }

    public override ValueType Type => ValueType.Void;

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