namespace MiniProgrammingLanguage.Core;

public readonly struct Location
{
    public static Location Default => new() { Line = 0, Position = 0, Filepath = string.Empty };

    public required int Line { get; init; }

    public required int Position { get; init; }

    public required string Filepath { get; init; }

    public override string ToString()
    {
        return $"at {Line} line, {Position} position in {Filepath}";
    }
}