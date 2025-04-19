using MiniProgrammingLanguage.Core.Lexer;

namespace MiniProgrammingLanguage.Core.Parser;

public readonly struct ParserConfiguration
{
    public required LexerConfiguration LexerConfiguration { get; init; }
}