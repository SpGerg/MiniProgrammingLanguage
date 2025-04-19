using MiniProgrammingLanguage.Core.Lexer.Enums;
using MiniProgrammingLanguage.Core.Parser;

namespace MiniProgrammingLanguage.Core.Lexer.Extensions;

public static class TokenTypesExtensions
{
    public static bool IsTypeToken(this TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.StringType => true,
            TokenType.BooleanType => true,
            TokenType.NumberType => true,
            TokenType.RoundNumberType => true,
            _ => false
        };
    }

    public static string GetTranslation(this TokenType tokenType, ParserConfiguration parserConfiguration)
    {
        return tokenType switch
        {
            TokenType.Word => "variable or function",
            TokenType.String => "string",
            TokenType.Number => "number",
            _ => parserConfiguration.LexerConfiguration.GetString(tokenType)
        };
    }
}