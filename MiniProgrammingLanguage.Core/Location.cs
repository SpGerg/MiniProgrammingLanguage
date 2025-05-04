namespace MiniProgrammingLanguage.Core;

public readonly struct Location
{
    /// <summary>
    /// Location with position and line at 0 and empty filepath
    /// </summary>
    public static Location Default => new() { Line = 0, Position = 0, Filepath = string.Empty };

    /// <summary>
    /// Line
    /// </summary>
    public required int Line { get; init; }

    /// <summary>
    /// Position
    /// </summary>
    public required int Position { get; init; }

    /// <summary>
    /// Filepath
    /// </summary>
    public required string Filepath { get; init; }

    /// <summary>
    /// Understable
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"at {Line} line, {Position} position in {Filepath}";
    }
}