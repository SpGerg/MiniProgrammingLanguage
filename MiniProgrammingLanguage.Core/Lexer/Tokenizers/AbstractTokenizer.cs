namespace MiniProgrammingLanguage.Core.Lexer.Tokenizers
{
    public abstract class AbstractTokenizer
    {
        protected AbstractTokenizer(Lexer lexer)
        {
            Lexer = lexer;
        }

        public Lexer Lexer { get; }

        public abstract Token Tokenize();
    }
}