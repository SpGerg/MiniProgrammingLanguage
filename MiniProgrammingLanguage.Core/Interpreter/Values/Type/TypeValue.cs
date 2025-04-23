using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeValue : AbstractValue
{
    public TypeValue(string name, IReadOnlyDictionary<ITypeMemberIdentification, TypeMemberValue> members) : base(name)
    {
        _members = members;
    }

    public override ValueType Type => ValueType.Type;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };

    public IReadOnlyDictionary<ITypeMemberIdentification, TypeMemberValue> Members => _members;

    private readonly IReadOnlyDictionary<ITypeMemberIdentification, TypeMemberValue> _members;
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.Number.ToString(), location);

        return -1;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.RoundNumber.ToString(), location);

        return -1;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.Boolean.ToString(), location);

        return false;
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"({Name}) ");
        stringBuilder.Append("{ ");

        foreach (var member in _members)
        {
            var value = member.Value.Value;
            
            stringBuilder.Append($"{member.Key.Identifier}: {(value is NoneValue ? "none" : value.AsString(programContext, location))}");
            
            if (member.Value == _members.Last().Value)
            {
                continue;
            }
            
            stringBuilder.Append(", ");
        }
        
        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }

    public TypeMemberValue Get(ITypeMemberIdentification typeMemberIdentification)
    {
        return Members.FirstOrDefault(member => member.Key.Is(typeMemberIdentification)).Value;
    }
}