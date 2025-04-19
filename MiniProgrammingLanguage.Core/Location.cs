namespace MiniProgrammingLanguage.Core;

public readonly struct Location
{
    public static Location Default => new() { Line = 0, Position = 0 };
    
    public required int Line { get; init; }
    
    public required int Position { get; init; }

    public override string ToString()
    {
        return $"at {Line} line, {Position} position";
    }
}