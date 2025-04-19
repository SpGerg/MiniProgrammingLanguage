using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class FunctionCallExpression : AbstractEvaluableExpression, IStatement
{
    public FunctionCallExpression(string name, AbstractEvaluableExpression[] arguments, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Arguments = arguments;
        Root = root;
    }
    
    public string Name { get; }
    
    public AbstractEvaluableExpression[] Arguments { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var function = programContext.Functions.Get(Root, Name, Location);

        if (function is null)
        {
            InterpreterThrowHelper.ThrowFunctionNotFoundException(Name, Location);

            return null;
        }

        if (function.IsAsync)
        {
            var context = new FunctionExecuteContext
            {
                ProgramContext = programContext,
                Arguments = Arguments,
                Location = Location
            };

            Task<AbstractValue> task = null;
            
            task = Task.Run(() =>
            {
                var result = function.Evaluate(context);

                programContext.Tasks.Remove(new TaskInstance
                {
                    Task = task,
                    Token = context.TokenSource
                });

                return result;
            });
            
            programContext.Tasks.Add(new TaskInstance
            {
                Task = task,
                Token = context.TokenSource
            });

            var taskType = context.ProgramContext.Types.Get(null, "__task", Location);

            if (taskType is null)
            {
                InterpreterThrowHelper.ThrowTypeNotFoundException(Name, Location);
            }

            var members = new Dictionary<ITypeMemberIdentification, AbstractValue>
            {
                { new KeyTypeMemberIdentification { Identificator = "id" }, new NumberValue(task.Id) }
            };
            
            var taskTypeValue = taskType.Create(members);
            
            return taskTypeValue;
        }

        return function.Evaluate(new FunctionExecuteContext
        {
            ProgramContext = programContext, 
            Arguments = Arguments,
            Location = Location
        });
    }
}