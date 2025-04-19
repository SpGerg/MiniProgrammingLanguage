namespace MiniProgrammingLanguage.Core.Interpreter.Values.Type;

public class TypeMemberValue
{
    public required AbstractValue Value { get; set; }
    
    public ObjectTypeValue Type { get; init; } = ObjectTypeValue.Any;
}