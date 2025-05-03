using System;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;

public class EnumMemberValue : AbstractValue
{
    public EnumMemberValue(IEnumInstance parent, string member) : base(parent is null ? string.Empty : parent.Name)
    {
        Parent = parent;
        Member = member;
    }

    public override ValueType Type => ValueType.EnumMember;

    public override ValueType[] CanCast { get; } = { ValueType.String };
    
    public IEnumInstance Parent { get; }

    public string Member { get; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new EnumMemberValue(Parent, Member);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        return $"({Name}) {Member}";
    }
}