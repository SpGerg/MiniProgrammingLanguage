using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Extensions;
using MiniProgrammingLanguage.Core.Lexer.Enums;
using MiniProgrammingLanguage.Core.Lexer.Tokenizers;

namespace MiniProgrammingLanguage.Core.Lexer
{
    public class Lexer
    {
        /// <summary>
        /// Create instance of lexer
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="filepath">Filepath</param>
        /// <param name="configuration">Keywords</param>
        public Lexer(string source, string filepath, LexerConfiguration configuration)
        {
            Source = source;
            Filepath = filepath;
            Configuration = configuration;

            _stringTokenizer = new StringTokenizer(this);
            _numberTokenizer = new NumberTokenizer(this);
        }

        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Filepath to script.
        /// Uses in errors.
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        /// Configuration
        /// </summary>
        public LexerConfiguration Configuration { get; }

        /// <summary>
        /// Current symbol
        /// </summary>
        public char Current => Source[Position];

        /// <summary>
        /// Previous symbol
        /// </summary>
        public char Previous => Source[Position - 1];

        /// <summary>
        /// Current position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Is not ended
        /// </summary>
        public bool IsNotEnded => Position < Source.Length;

        /// <summary>
        /// Is ended
        /// </summary>
        public bool IsEnded => !IsNotEnded;

        private readonly StringTokenizer _stringTokenizer;

        private readonly NumberTokenizer _numberTokenizer;

        /// <summary>
        /// Tokenized tokens based on configuration
        /// </summary>
        /// <returns></returns>
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

                if (string.IsNullOrEmpty(buffer) && _numberTokenizer.TryGetDigit(out _))
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

            if (!string.IsNullOrWhiteSpace(buffer) && !string.IsNullOrWhiteSpace(buffer))
            {
                tokens.Add(new Token
                {
                    Type = TokenType.Word,
                    Value = buffer,
                    Location = Source.GetLocationByPosition(position, Filepath)
                });
            }

            return tokens;
        }

        public void Skip()
        {
            Position++;
        }
    }
}