using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Extensions;
using MiniProgrammingLanguage.Core.Lexer.Enums;
using MiniProgrammingLanguage.Core.Lexer.Tokenizers;

namespace MiniProgrammingLanguage.Core.Lexer
{
    public class Lexer
    {
        public Lexer(string source, string filepath, LexerConfiguration configuration)
        {
            Source = source;
            Filepath = filepath;
            Configuration = configuration;

            _stringTokenizer = new StringTokenizer(this);
            _numberTokenizer = new NumberTokenizer(this);
        }
        
        public string Source { get; }
        
        public string Filepath { get; }
        
        public LexerConfiguration Configuration { get; }

        public char Current => Source[Position];
        
        public char Previous => Source[Position - 1];
        
        public int Position { get; set; }

        public bool IsNotEnded => Position < Source.Length;
        
        public bool IsEnded => !IsNotEnded;

        private readonly StringTokenizer _stringTokenizer;

        private readonly NumberTokenizer _numberTokenizer;

        public IReadOnlyList<Token> Tokenize()
        {
            var tokens = new List<Token>();
            
            var buffer = string.Empty;
            var position = Position + 1;

            while (IsNotEnded)
            {
                if (char.IsControl(Current))
                {
                    if (!string.IsNullOrWhiteSpace(buffer) && !string.IsNullOrWhiteSpace(buffer))
                    {
                        tokens.Add(new Token
                        {
                            Type = TokenType.Word,
                            Value = buffer,
                            Location = Source.GetLocationByPosition(position, Filepath)
                        });
                    }
                    
                    buffer = string.Empty;
                    position = Position + 1;
                    
                    Skip();

                    continue;
                } 
                
                if (_numberTokenizer.TryGetDigit(out _))
                {
                    tokens.Add(_numberTokenizer.Tokenize());
                    
                    continue;
                }

                if (_stringTokenizer.IsQuote())
                {
                    tokens.Add(_stringTokenizer.Tokenize());
                    
                    continue;
                }
                
                var current = Current;

                buffer += current;
                Skip();

                if (IsEnded)
                {
                    current = Previous;
                }

                if (current.IsSpace())
                {
                    if (string.IsNullOrWhiteSpace(buffer))
                    {
                        buffer = string.Empty;
                        position = Position + 1;
                        
                        continue;
                    }
                    
                    var result = buffer.Remove(buffer.Length - 1);
                    var token = Configuration.IsToken(result);
                    
                    tokens.Add(new Token
                    {
                        Type = token is TokenType.None ? TokenType.Word : token,
                        Value = result,
                        Location = Source.GetLocationByPosition(position, Filepath)
                    });
                    
                    buffer = string.Empty;
                    position = Position + 1;
                    
                    continue;
                }

                var operatorToken = Configuration.IsToken(current);

                if (operatorToken is TokenType.None)
                {
                    var token = Configuration.IsToken(buffer);

                    if (token is TokenType.None)
                    {
                        continue;
                    }

                    if (IsNotEnded && !char.IsWhiteSpace(Current))
                    {
                        continue;
                    }

                    tokens.Add(new Token
                    {
                        Type = token,
                        Value = buffer,
                        Location = Source.GetLocationByPosition(position, Filepath)
                    });
                    
                    buffer = string.Empty;
                    position = Position + 1;
                    
                    continue;
                }

                var currentString = current.ToString();
                
                if (currentString != buffer)
                {
                    var result = buffer.Remove(buffer.Length - 1);
                    var token = Configuration.IsToken(result);
                        
                    tokens.Add(new Token
                    {
                        Type = token is TokenType.None ? TokenType.Word : token,
                        Value = result,
                        Location = Source.GetLocationByPosition(position, Filepath)
                    });
                }
                    
                tokens.Add(new Token
                {
                    Type = operatorToken,
                    Value = currentString,
                    Location = Source.GetLocationByPosition(position, Filepath)
                });
                    
                buffer = string.Empty;
                position = Position + 1;
            }

            return tokens;
        }

        public void Skip()
        {
            Position++;
        }
    }
}