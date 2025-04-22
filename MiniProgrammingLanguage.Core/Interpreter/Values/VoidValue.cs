using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class VoidValue : AbstractValue
{
    public override ValueType Type => ValueType.Void;
    
    public override ValueType[] CanCast { get; } = {};
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }
}