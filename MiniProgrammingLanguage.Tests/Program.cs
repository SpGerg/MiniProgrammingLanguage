using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Lexer;
using MiniProgrammingLanguage.Core.Parser;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "script2.mpl");
            var lexer = new Lexer(
                File.ReadAllText(filepath),
                filepath,
                LexerConfiguration.Default);
            var tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            var parser = new Parser(tokens, filepath, new ParserConfiguration
            {
                LexerConfiguration = lexer.Configuration
            });

            var functionBody = parser.Parse();

            var printFunction = new LanguageFunctionInstance
            {
                Name = "print",
                Arguments = new[] { new FunctionArgument("content") },
                Bind = Print,
                Return = new ObjectTypeValue(string.Empty, ValueType.Any),
                IsAsync = false,
                Root = null
            };
            
            var sleepFunction = new LanguageFunctionInstance
            {
                Name = "sleep",
                Bind = Sleep,
                Arguments = new[] { new FunctionArgument("milliseconds", ObjectTypeValue.RoundNumber) },
                Return = ObjectTypeValue.Void,
                IsAsync = false,
                Root = null
            };

            var moduleFunction = new LanguageVariableInstance
            {
                Name = "__module",
                Bind = GetModule,
                ObjectType = ObjectTypeValue.String,
                Root = null
            };

            var taskInstance = new UserTypeInstance
            {
                Name = "__task",
                Members = new List<ITypeMember>
                {
                    new TypeVariableMemberInstance
                    {
                        Parent = "__task",
                        Identification = new KeyTypeMemberIdentification
                        {
                            Identifier = "id"
                        },
                        Default = new RoundNumberValue(-1),
                        IsReadonly = true,
                        Type = new ObjectTypeValue(string.Empty, ValueType.RoundNumber)
                    }
                },
                Root = null
            };

            var programContext = new ProgramContext(filepath, new [] { taskInstance }, new[] { printFunction, sleepFunction }, new [] { moduleFunction });
            functionBody.Evaluate(programContext);

            while (programContext.Tasks.Entities.Any())
            {
            }
            
            programContext.Tasks.Clear();
            programContext.Variables.Clear();
            programContext.Functions.Clear();
            programContext.Types.Clear();

            Console.ReadLine();
        }

        private static AbstractValue Print(FunctionExecuteContext context)
        {
            var argument = context.Arguments[0];
            var content = argument.Evaluate(context.ProgramContext);

            var message = string.Empty;

            if (content is NoneValue noneValue)
            {
                message = "none";
            }
            else
            {
                message = content.AsString(context.ProgramContext, context.Location);
            }
            
            Console.WriteLine(message);
            
            return new VoidValue();
        }

        private static AbstractValue Sleep(FunctionExecuteContext functionExecuteContext)
        {
            var milliseconds = functionExecuteContext.Arguments[0];
            var time = milliseconds.Evaluate(functionExecuteContext.ProgramContext)
                .AsRoundNumber(functionExecuteContext.ProgramContext, functionExecuteContext.Location);

            functionExecuteContext.Token.WaitHandle.WaitOne(time);

            return new VoidValue();
        }

        private static AbstractValue GetModule(VariableGetterContext context)
        {
            return new StringValue(context.ProgramContext.Module);
        }
    }
}