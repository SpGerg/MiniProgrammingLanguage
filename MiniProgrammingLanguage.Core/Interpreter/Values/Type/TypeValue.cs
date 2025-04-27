using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeValue : AbstractValue
{
    public TypeValue(string name, ITypeInstance value, IReadOnlyDictionary<ITypeMemberIdentification, ITypeMemberValue> members) : base(name)
    {
        Members = members;
        Value = value;
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
        return new TypeValue(Name, Value, Members);
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

        foreach (var member in Members)
        {
            var context = new TypeMemberGetterContext
            {
                ProgramContext = programContext,
                Type = this,
                Member = member.Value.Instance,
                Location = location
            };
            
            var value = member.Value.GetValue(context);
            
            stringBuilder.Append($"{member.Key.Identifier}: {(value is NoneValue ? "none" : value.AsString(programContext, location))}");
            
            if (member.Value == Members.Last().Value)
            {
                continue;
            }
            
            stringBuilder.Append(", ");
        }
        
        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }

    public ITypeMemberValue Get(ITypeMemberIdentification typeMemberIdentification)
    {
        return Members.FirstOrDefault(member => member.Key.Is(typeMemberIdentification)).Value;
    }
}