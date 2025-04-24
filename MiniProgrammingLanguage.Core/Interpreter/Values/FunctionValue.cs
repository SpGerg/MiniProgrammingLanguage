using System.Text;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public class FunctionValue : AbstractValue
{
    public FunctionValue(IFunctionInstance value) : base(string.Empty)
    {
        Value = value;
    }

    public override ValueType Type => ValueType.Function;

    public override ValueType[] CanCast { get; } = { ValueType.String };
    
    public IFunctionInstance Value { get; set; }
    
    public override bool Visit(IValueVisitor visitor)
    {
        return visitor.Visit(this);
    }
    
    public override AbstractValue Copy()
    {
        return new FunctionValue(Value);
    }
    
    public override string AsString(ProgramContext programContext, Location location)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("{ " + $"name: {Value.Name}, async: {Value.IsAsync}, declared: {Value.IsDeclared}, return: {Value.Return}, arguments: ");

        foreach (var argument in Value.Arguments)
        {
            stringBuilder.Append(argument);
        }

        stringBuilder.Append(" }");

        return stringBuilder.ToString();
    }
}