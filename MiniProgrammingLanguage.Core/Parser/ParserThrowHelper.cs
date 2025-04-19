using System.Linq;
using System.Text;
using MiniProgrammingLanguage.Core.Lexer.Enums;
using MiniProgrammingLanguage.Core.Lexer.Extensions;
using MiniProgrammingLanguage.Core.Parser.Exceptions;

namespace MiniProgrammingLanguage.Core.Parser;

public static class ParserThrowHelper
{
    public static void ThrowTokenExpectedException(ParserConfiguration configuration, Location location, params TokenType[] tokenType)
    {
        if (tokenType.Length is 1)
        {
            var token = tokenType.First();
            var message = token.GetTranslation(configuration);
            
            throw new TokenExpectedException($"'{message}'", location);
        }
        
        var stringBuilder = new StringBuilder();
        
        foreach (var token in tokenType)
        {
            var message = token.GetTranslation(configuration);

            if (token == tokenType.Last())
            {
                stringBuilder.Append($"or '{message}'");
                
                break;
            }
            
            stringBuilder.Append($"'{message}', ");
        }

        throw new TokenExpectedException(stringBuilder.ToString(), location);
    }
    
    public static void ThrowValueExceptedException(Location location) 
    {
        throw new ValueExpectedException(location);
    }
    
    public static void ThrowTypeExceptedException(Location location)
    {
        throw new TypeExpectedException(location);
    }

    public static void ThrowInvalidNumberFormatException(string number, Location location)
    {
        throw new InvalidNumberFormatException(number, location);
    }
    
    public static void ThrowStatementExceptedException(Location location)
    {
        throw new StatementExceptedException(location);
    }
}