using MiniProgrammingLanguage.Core.Lexer.Enums;

namespace MiniProgrammingLanguage.Core.Lexer
{
    public readonly struct Token
    {
        public required TokenType Type { get; init; }
        
        public required string Value { get; init; }
        
        public Location Location { get; init; }

        public override string ToString()
        {
            return $"Type: {Type.ToString()}, {Value}, {Location}";
        }
    }
}