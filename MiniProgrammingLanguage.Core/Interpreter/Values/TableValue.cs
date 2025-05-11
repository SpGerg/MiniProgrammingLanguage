using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class TableValue : AbstractDataContainerValue
{
    public TableValue(Dictionary<ITypeMemberIdentification, ITypeMemberValue> members) : base("table")
    {
        Members = members;
    }

    public override ValueType Type => ValueType.Table;

    public override ValueType[] CanCast { get; } =
    {
        ValueType.String
    };
    
    public Dictionary<ITypeMemberIdentification, ITypeMemberValue> Members { get; }

    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("{ ");

        foreach (var member in Members)
        {
            string value;

            if (member.Value is ITypeVariableMemberValue variableMemberValue)
            {
                var getterContext = new TypeMemberGetterContext
                {
                    ProgramContext = programContext,
                    Type = null,
                    Member = member.Value.Instance,
                    Location = location
                };
                
                value = variableMemberValue.GetValue(getterContext).AsString(programContext, location);
            }
            else
            {
                value = "function";
            }

            stringBuilder.Append($"{member.Key.Identifier} = {value}");

            if (member.Key != Members.Last().Key)
            {
                stringBuilder.Append(", ");
            }
        }
        
        stringBuilder.Append(" }");
        return stringBuilder.ToString();
    }

    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }

    public override AbstractValue Copy()
    {
        return new TableValue(Members);
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
            Type = null,
            Value = abstractValue,
            Location = location
        };

        typeMemberValue.SetValue(setterContext);
    }
}