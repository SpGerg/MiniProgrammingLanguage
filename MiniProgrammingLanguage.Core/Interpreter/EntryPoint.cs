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
        public EntryPoint(string filepath)
        {
            Filepath = filepath;
        }

        public string Filepath { get; set; }

        /// <summary>
        /// Tokenize, parse and interpreter source in the script by filepath.
        /// If after execution some tasks still in progress, the thread where this function was executed will be frozen.
        /// </summary>
        /// <param name="exception">Exception without stack trace</param>
        /// <param name="modules">Default modules</param>
        /// <returns></returns>
        public AbstractValue Run(out AbstractLanguageException exception, params ImplementModule[] modules)
        {
            var content = File.ReadAllText(Filepath);
            var lexer = new Lexer.Lexer(content, Filepath, LexerConfiguration.Default);
            var tokens = lexer.Tokenize();

            var parser = new Parser.Parser(tokens, Filepath, new ParserConfiguration
            {
                LexerConfiguration = lexer.Configuration
            });
            var functionBodyExpression = parser.Parse();

            AbstractValue result = null;

            var programContext = new ProgramContext(Filepath, modules);

            try
            {
                result = functionBodyExpression.Evaluate(programContext);

                while (programContext.Tasks.Entities.Any())
                {
                }

                programContext.Tasks.Clear();
                programContext.Variables.Clear();
                programContext.Functions.Clear();
                programContext.Types.Clear();

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