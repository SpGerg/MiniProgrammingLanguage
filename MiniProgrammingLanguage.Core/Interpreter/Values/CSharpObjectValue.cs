using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class CSharpObjectValue : AbstractValue
{
    public CSharpObjectValue(object value) : base(value.GetType().Name)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.CSharpObject;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };

    public override bool IsValueType => false;

    public object Value { get; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new CSharpObjectValue(Value);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return Value.ToString();
    }
}