using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;

public class EnumMemberValue : AbstractValue
{
    public EnumMemberValue(IEnumInstance parent, string member, int value) : base(parent is null ? string.Empty : parent.Name)
    {
        Parent = parent;
        Member = member;
        Value = value;
    }

    public override ValueType Type => ValueType.EnumMember;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String,
        ValueType.RoundNumber,
        ValueType.Number
    };
    
    public IEnumInstance Parent { get; }

    public string Member { get; }
    
    public int Value { get; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new EnumMemberValue(Parent, Member, Value);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return Member;
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        return Value;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        return Value;
    }
}