using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignArrayMemberExpression : AbstractEvaluableExpression, IStatement
{
    public AssignArrayMemberExpression(VariableExpression array, AbstractEvaluableExpression index, AbstractEvaluableExpression value, Location location) : base(location)
    {
        Array = array;
        Index = index;
        Value = value;
    }
    
    public VariableExpression Array { get; }
    
    public AbstractEvaluableExpression Index { get; }
    
    public AbstractEvaluableExpression Value { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var index = Index.Evaluate(programContext).AsRoundNumber(programContext, Location);

        if (index < 0)
        {
            InterpreterThrowHelper.ThrowCannotAccessException("array", Location);
        }
        
        var array = Array.Evaluate(programContext);

        if (array is not ArrayValue arrayValue)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Array.ToString(), array.ToString(), Location);
            return null;
        }

        if (index > arrayValue.Value.Length - 1)
        {
            InterpreterThrowHelper.ThrowCannotAccessException("array", Location);
        }

        arrayValue.Value[index] = Value;

        return new VoidValue();
    }
}