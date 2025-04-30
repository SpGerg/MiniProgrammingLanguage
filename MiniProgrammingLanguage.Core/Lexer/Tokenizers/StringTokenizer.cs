using MiniProgrammingLanguage.Core.Extensions;
using MiniProgrammingLanguage.Core.Lexer.Enums;

namespace MiniProgrammingLanguage.Core.Lexer.Tokenizers;

public class StringTokenizer : AbstractTokenizer
{
    public StringTokenizer(Lexer lexer) : base(lexer)
    {
    }

    public override Token Tokenize()
    {
        //If tokenizing started at start quote, we will skip
        if (IsQuote())
        {
            Lexer.Skip();
        }

        var buffer = string.Empty;

        while (Lexer.IsNotEnded)
        {
            if (IsQuote())
            {
                Lexer.Skip();

                break;
            }

            buffer += Lexer.Current;
            Lexer.Skip();
        }

        return new Token
        {
            Type = TokenType.String,
            Value = buffer,
            Location = Lexer.Source.GetLocationByPosition(Lexer.Position, Lexer.Filepath)
        };
    }

    public bool IsQuote()
    {
        return Lexer.Current is '\"';
    }
}