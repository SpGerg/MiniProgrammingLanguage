using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeInstanceValue : AbstractValue
{
    public TypeInstanceValue(ITypeInstance value) : base(value.Name)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.TypeInstance;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };
    
    public ITypeInstance Value { get; }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return ToString();
    }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new TypeInstanceValue(Value);
    }
}