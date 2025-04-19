using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AwaitExpression : AbstractEvaluableExpression, IStatement
{
    public AwaitExpression(FunctionCallExpression functionCallExpression, Location location) : base(location)
    {
        FunctionCall = functionCallExpression;
    }
    
    public FunctionCallExpression FunctionCall { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var value = FunctionCall.Evaluate(programContext);

        if (value is not TypeValue { Name: "__task" } typeValue)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException("__task", value.Type.ToString(), Location);

            return null;
        }

        var idMember = typeValue.Get(new KeyTypeMemberIdentification
        {
            Identificator = "id"
        });

        if (idMember?.Value is null)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(typeValue.Name, "id", Location);
        }

        var id = idMember.Value.AsRoundNumber(programContext, Location);

        var task = programContext.Tasks.Get(id);

        if (task is null)
        {
            InterpreterThrowHelper.ThrowCannotAccessException(typeValue.AsString(programContext, Location), Location);
        }

        task.Task.Wait();
        
        return task.Task.Result;
    }
}