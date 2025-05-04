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

    /// <summary>
    /// Function name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Argument type.
    /// By default - any.
    /// </summary>
    public ObjectTypeValue Type { get; }

    /// <summary>
    /// Default value
    /// </summary>
    public AbstractValue Default { get; }

    /// <summary>
    /// Is required.
    /// Required if default is null.
    /// </summary>
    public bool IsRequired => Default is null;

    /// <summary>
    /// Function argument in json like style
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "{ " + $"name: {Name}, type: {Type}, required: {IsRequired.ToString().ToLower()} " + "}";
    }
}