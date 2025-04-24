using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;

public class EnumMemberValue : AbstractValue
{
    public EnumMemberValue(string parent, string member) : base(parent)
    {
        Member = member;
    }

    public override ValueType Type => ValueType.EnumMember;

    public override ValueType[] CanCast { get; } = { ValueType.String };

    public string Member { get; }
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new EnumMemberValue(Name, Member);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return $"({Name}) {Member}";
    }
}