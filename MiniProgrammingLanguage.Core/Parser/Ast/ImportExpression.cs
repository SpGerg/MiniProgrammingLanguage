using System;
using System.IO;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Lexer;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ImportExpression : AbstractEvaluableExpression, IStatement
{
    public ImportExpression(AbstractEvaluableExpression filepath, Location location) : base(location)
    {
        Filepath = filepath;
    }
    
    public AbstractEvaluableExpression Filepath { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var directory = Path.GetDirectoryName(programContext.Filepath);
        var content = string.Empty;

        if (Filepath.IsValue)
        {
            content = Filepath.Evaluate(programContext).AsString(programContext, Location);
        }
        else
        {
            if (Filepath is VariableExpression variableExpression)
            {
                var variable = programContext.Variables.Get(variableExpression.Root, variableExpression.Name, variableExpression.Location);

                if (variable is null)
                {
                    content = variableExpression.Name;
                }
                else
                {
                    content = variable.GetValue(new VariableGetterContext
                    {
                        ProgramContext = programContext,
                        Location = variableExpression.Location
                    }).AsString(programContext, Location);
                }
            }
            else
            {
                content = Filepath.Evaluate(programContext).AsString(programContext, Location);
            }
        }

        var filepath = Path.Combine(directory, $"{content}.mpl");
        
        var source = string.Empty;

        try
        {
            source = File.ReadAllText(filepath);
        }
        catch
        {
            InterpreterThrowHelper.ThrowWrongImportModuleException(content, Location);
        }

        var module = new ImplementModule
        {
            Name = programContext.Module,
            Types = programContext.Types.Entities,
            Functions = programContext.Functions.Entities,
            Enums = programContext.Enums.Entities,
            Variables = programContext.Variables.Entities,
            Location = Location
        };
        var moduleContext = new ProgramContext(filepath, module);

        var lexer = new Lexer.Lexer(source, filepath, LexerConfiguration.Default);
        var tokens = lexer.Tokenize();

        var parser = new Parser(tokens, filepath, new ParserConfiguration
        {
            LexerConfiguration = lexer.Configuration
        });
        
        var expressions = parser.Parse();
        expressions.Evaluate(moduleContext);
        
        programContext.Import(moduleContext, Location);

        return new VoidValue();
    }
}