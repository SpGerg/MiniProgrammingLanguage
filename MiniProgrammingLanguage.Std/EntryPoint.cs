using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Lexer;
using MiniProgrammingLanguage.Core.Parser;

namespace MiniProgrammingLanguage.Std
{
    public class EntryPoint
    {
        public EntryPoint(string filepath)
        {
            Filepath = filepath;
        }
        
        public string Filepath { get; set; }

        public AbstractValue Run(out AbstractLanguageException exception,
            IEnumerable<ITypeInstance> types = null, IEnumerable<IFunctionInstance> functions = null, IEnumerable<IEnumInstance> enums = null, IEnumerable<IVariableInstance> variables = null)
        {
            var content = File.ReadAllText(Filepath);
            var lexer = new Lexer(content, Filepath, LexerConfiguration.Default);
            var tokens = lexer.Tokenize();

            var parser = new Parser(tokens, Filepath, new ParserConfiguration
            {
                LexerConfiguration = lexer.Configuration
            });
            var functionBodyExpression = parser.Parse();

            AbstractValue result = null;

            var programContext = new ProgramContext(Filepath, types, functions, enums, variables);

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