using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class VariableSetterContext
{
    public required ProgramContext ProgramContext { get; init; }
    
    public required AbstractValue Value { get; init; }
    
    public Location Location { get; init; }
}