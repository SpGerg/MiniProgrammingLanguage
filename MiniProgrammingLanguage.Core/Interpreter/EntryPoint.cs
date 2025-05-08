using System.IO;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Lexer;
using MiniProgrammingLanguage.Core.Parser;

namespace MiniProgrammingLanguage.Core.Interpreter
{
    public class EntryPoint
    {
        public EntryPoint(string executor, string source = null)
        {
            Executor = executor;
            Source = source;
        }

        public string Executor { get; set; }
        
        public string Source { get; set; }

        /// <summary>
        /// Tokenize, parse and interpreter source in the script by filepath.
        /// If after execution some tasks still in progress, the thread where this function was executed will be frozen.
        /// </summary>
        /// <param name="exception">Exception without stack trace</param>
        /// <param name="modules">Default modules</param>
        /// <returns></returns>
        public AbstractValue Run(out ProgramContext programContext, out AbstractLanguageException exception,  bool isClear = true, params ImplementModule[] modules)
        {
            var content = Source ?? File.ReadAllText(Executor);
            var lexer = new Lexer.Lexer(content, Executor, LexerConfiguration.Default);
            var tokens = lexer.Tokenize();

            var parser = new Parser.Parser(tokens, Executor, new ParserConfiguration
            {
                LexerConfiguration = lexer.Configuration
            });
            var functionBodyExpression = parser.Parse();

            AbstractValue result = null;

            programContext = new ProgramContext(Executor, modules);

            try
            {
                result = functionBodyExpression.Evaluate(programContext);

                while (programContext.Tasks.Entities.Any())
                {
                }

                if (isClear)
                {
                    programContext.Tasks.Clear();
                    programContext.Variables.Clear();
                    programContext.Functions.Clear();
                    programContext.Types.Clear();
                }

                exception = null;
            }
            catch (AbstractLanguageException abstractLanguageException)
            {
                exception = abstractLanguageException;
            }

            return result;
        }
    }
}