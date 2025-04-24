using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;

public class EnumValue : AbstractValue
{
    public EnumValue(IEnumInstance value) : base(value.Name)
    {
        Value = value;

        _last = Value.Members.Last();
    }

    public override ValueType Type => ValueType.Enum;

    public override ValueType[] CanCast { get; } = { ValueType.String };
    
    public IEnumInstance Value { get; }

    private KeyValuePair<string, int> _last;

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new EnumValue(Value);
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"({Value.Name}) " + "{ ");

        foreach (var member in Value.Members)
        {
            stringBuilder.Append(member.Key);

            if (member.Key == _last.Key && member.Value == _last.Value)
            {
                continue;
            }
            
            stringBuilder.Append(", ");
        }

        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }
}