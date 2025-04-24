using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Lexer.Enums;

namespace MiniProgrammingLanguage.Core.Lexer;

public readonly struct LexerConfiguration
{
    public static LexerConfiguration Default { get; } = new()
    {
        Configuration = new Dictionary<TokenType, string>
        {
            { TokenType.For, "for" },
            { TokenType.In, "in" },
            { TokenType.True, "true" },
            { TokenType.False, "false" },
            { TokenType.LeftParentheses, "(" },
            { TokenType.RightParentheses, ")" },
            { TokenType.LeftBrace, "{" },
            { TokenType.RightBrace, "}" },
            { TokenType.LeftSquareBracket, "[" },
            { TokenType.RightSquareBracket, "]" },
            { TokenType.Dot, "." },
            { TokenType.Colon, ":" },
            { TokenType.Comma, "," },
            { TokenType.Multiplication, "*" },
            { TokenType.Division, "/" },
            { TokenType.Plus, "+" },
            { TokenType.Minus, "-" },
            { TokenType.Greater, ">" },
            { TokenType.Less, "<" },
            { TokenType.Function, "function" },
            { TokenType.Async, "async" },
            { TokenType.End, "end" },
            { TokenType.Return, "return" },
            { TokenType.StringType, "string" },
            { TokenType.NumberType, "number" },
            { TokenType.RoundNumberType, "round_number" },
            { TokenType.BooleanType, "boolean" },
            { TokenType.Is, "is" },
            { TokenType.If, "if" },
            { TokenType.Then, "then" },
            { TokenType.Else, "else" },
            { TokenType.Module, "module" },
            { TokenType.NoneValue, "none" },
            { TokenType.Equals, "=" },
            { TokenType.Type, "type" },
            { TokenType.Create, "create" },
            { TokenType.Await, "await" },
            { TokenType.And, "and" },
            { TokenType.Or, "or" },
            { TokenType.While, "while" },
            { TokenType.Break, "break" },
            { TokenType.Import, "import" },
            { TokenType.Implement, "implement" },
            { TokenType.Enum, "enum" },
            { TokenType.EnumMember, "enum_member" },
            { TokenType.Static, "static" },
            { TokenType.ReadOnly, "readonly" },
            { TokenType.Call, "call" }
        }
    };
    
    public IReadOnlyDictionary<TokenType, string> Configuration { get; init; }

    public string GetString(TokenType tokenType)
    {
        return Configuration[tokenType];
    }

    public TokenType IsToken(char value) => IsToken(value.ToString());
    
    public TokenType IsToken(string value)
    {
        var result = Configuration.FirstOrDefault(pair => pair.Value == value);

        //result == default
        if (result.Equals(default(KeyValuePair<TokenType, string>)))
        {
            return TokenType.None;
        }
        
        return result.Key;
    }
}