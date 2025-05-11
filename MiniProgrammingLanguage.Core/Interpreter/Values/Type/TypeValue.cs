using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeValue : AbstractDataContainerValue
{
    public TypeValue(ITypeInstance value, IReadOnlyDictionary<ITypeMemberIdentification, ITypeMemberValue> members) :
        base(value.Name)
    {
        Members = members;
        Value = value;
    }

    public TypeValue(ITypeInstance value) : this(value, value.Create().Members)
    {
    }

    public override ValueType Type => ValueType.Type;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };

    public override bool IsValueType => false;

    public ITypeInstance Value { get; }

    public IReadOnlyDictionary<ITypeMemberIdentification, ITypeMemberValue> Members { get; }

    public object ObjectTarget { get; set; }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new TypeValue(Value, Members);
    }

    public override float AsNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.Number.ToString(),
            location);

        return -1;
    }

    public override int AsRoundNumber(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.RoundNumber.ToString(),
            location);

        return -1;
    }

    public override bool AsBoolean(ProgramContext programContext, Location location)
    {
        InterpreterThrowHelper.ThrowCannotCastException(ValueType.Type.ToString(), ValueType.Boolean.ToString(),
            location);

        return false;
    }

    public override string AsString(ProgramContext programContext, Location location)
    {
        /*
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"({Name}) ");
        stringBuilder.Append("{ ");

        foreach (var member in Members)
        {
            AbstractValue value;

            if (member.Value is ITypeVariableMemberValue variableMember)
            {
                var context = new TypeMemberGetterContext
                {
                    ProgramContext = programContext,
                    Type = this,
                    Member = variableMember.Instance,
                    Location = location
                };

                value = variableMember.GetValue(context);
            }
            else
            {
                value = member.Value.Type;
            }

            stringBuilder.Append(
                $"{member.Key.Identifier}: {(value is NoneValue ? "none" : value.AsString(programContext, location))}");

            if (member.Value == Members.Last().Value)
            {
                continue;
            }

            stringBuilder.Append(", ");
        }

        stringBuilder.Append(" }");

        return stringBuilder.ToString();
        */

        return Name;
    }

    public override ITypeMemberValue Get(ITypeMemberIdentification identification)
    {
        var member = Members.FirstOrDefault(member => member.Key.Is(identification));

        return member.Equals(default) ? null : member.Value;
    }

    public override void Set(ProgramContext programContext, ITypeMemberIdentification identification, AbstractValue abstractValue, Location location)
    {
        var member = Get(identification);

        if (member is not ITypeVariableMemberValue typeMemberValue)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(Name, identification.Identifier, location);
            return;
        }
        
        var setterContext = new TypeMemberSetterContext
        {
            ProgramContext = programContext,
            Member = member.Instance,
            Type = this,
            Value = abstractValue,
            Location = location
        };

        typeMemberValue.SetValue(setterContext);
    }
}