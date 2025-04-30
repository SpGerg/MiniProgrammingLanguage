namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;

public class VariableGetterContext
{
    public required ProgramContext ProgramContext { get; init; }

    public Location Location { get; init; }
}