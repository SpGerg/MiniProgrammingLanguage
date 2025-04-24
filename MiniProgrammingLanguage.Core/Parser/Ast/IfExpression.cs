using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class IfExpression : AbstractEvaluableExpression, IStatement, IControlFlowStatement
{
    public IfExpression(AbstractEvaluableExpression condition, FunctionBodyExpression body, FunctionBodyExpression elseBody, Location location) : base(location)
    {
        Condition = condition;
        Body = body;
        ElseBody = elseBody;
    }
    
    public AbstractEvaluableExpression Condition { get; }
    
    public FunctionBodyExpression Body { get; }
    
    public FunctionBodyExpression ElseBody { get; }

    public StateType State { get; private set; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        AbstractValue result;

        if (Condition is VariableExpression variableExpression)
        {
            var variable = programContext.Variables.Get(variableExpression.Root, variableExpression.Name, programContext.Module, Location);
            
            result = new BooleanValue(variable is not null);
        }
        else
        {
            result = Condition.Evaluate(programContext);
        }
        
        if (result is not BooleanValue booleanValue)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Boolean.ToString(), result.Type.ToString(), Location);

            return null;
        }

        if (booleanValue.Value)
        {
            var bodyResult = Body.Evaluate(programContext);

            State = Body.State;
            return bodyResult;
        }

        if (ElseBody is not null)
        {
            var elseResult = ElseBody.Evaluate(programContext);

            State = ElseBody.State;
            return elseResult;
        }

        return new VoidValue();
    }
}