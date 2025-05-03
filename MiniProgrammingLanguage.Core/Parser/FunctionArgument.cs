using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;

namespace MiniProgrammingLanguage.Core.Parser;

public class FunctionArgument
{
    public FunctionArgument(string name, ObjectTypeValue objectTypeValue = null, AbstractValue defaultValue = null)
    {
        Name = name;
        Default = defaultValue;
        Type = objectTypeValue ?? new ObjectTypeValue(string.Empty, ValueType.Any);
    }

    public string Name { get; }

    public ObjectTypeValue Type { get; }

    public AbstractValue Default { get; }

    public bool IsRequired => Default is null;

    public override string ToString()
    {
        return "{" + $"name: {Name}, type: {Type}, required: {IsRequired.ToString().ToLower()} " + "}";
    }
}