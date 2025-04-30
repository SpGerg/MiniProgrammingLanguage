using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class FunctionCallExpression : AbstractEvaluableExpression, IStatement
{
    public FunctionCallExpression(string name, AbstractEvaluableExpression[] arguments, FunctionBodyExpression root,
        Location location) : base(location)
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
        var function = programContext.Functions.Get(Root, Name, programContext.Module, Location);

        if (function is null)
        {
            var variable = programContext.Variables.Get(Root, Name, programContext.Module, Location);

            if (variable is null)
            {
                InterpreterThrowHelper.ThrowFunctionNotFoundException(Name, Location);
            }

            var value = variable.GetValue(new VariableGetterContext
            {
                ProgramContext = programContext,
                Location = Location
            });

            if (value is not FunctionValue functionValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Function.ToString(), value.ToString(),
                    Location);

                return null;
            }

            function = functionValue.Value;
        }

        return Call(programContext, function);
    }

    public AbstractValue Call(ProgramContext programContext, IFunctionInstance function)
    {
        if (function.IsAsync)
        {
            var context = new FunctionExecuteContext
            {
                ProgramContext = programContext,
                Root = Root,
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

            var taskType = context.ProgramContext.Types.Get(null, "__task", programContext.Module, Location);

            if (taskType is null)
            {
                InterpreterThrowHelper.ThrowTypeNotFoundException(Name, Location);
            }

            var taskTypeValue = taskType.Create();

            var identification = new KeyTypeMemberIdentification { Identifier = "id" };
            var member = (TypeMemberValue)taskTypeValue.Get(identification);

            member.Value = new NumberValue(task.Id);

            return taskTypeValue;
        }

        return function.Evaluate(new FunctionExecuteContext
        {
            ProgramContext = programContext,
            Root = Root,
            Arguments = Arguments,
            Location = Location
        });
    }
}