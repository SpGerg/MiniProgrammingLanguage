using MiniProgrammingLanguage.Core.Extensions;
using MiniProgrammingLanguage.Core.Lexer.Enums;

namespace MiniProgrammingLanguage.Core.Lexer.Tokenizers;

public class NumberTokenizer : AbstractTokenizer
{
    public NumberTokenizer(Lexer lexer) : base(lexer)
    {
    }

    public override Token Tokenize()
    {
        var buffer = string.Empty;
        var position = Lexer.Position;

        while (Lexer.IsNotEnded)
        {
            if (Lexer.Current is '.')
            {
                buffer += ',';
                Lexer.Skip();

                continue;
            }

            if (TryGetDigit(out var digit))
            {
                buffer += digit;
                Lexer.Skip();

                continue;
            }

            break;
        }

        return new Token
        {
            Type = TokenType.Number,
            Value = buffer,
            Location = Lexer.Source.GetLocationByPosition(position, Lexer.Filepath)
        };
    }

    public bool TryGetDigit(out char digit)
    {
        digit = Lexer.Current;

        return int.TryParse(digit.ToString(), out _);
    }
}