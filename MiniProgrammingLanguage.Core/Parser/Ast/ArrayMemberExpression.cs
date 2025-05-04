using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ArrayMemberExpression : AbstractEvaluableExpression
{
    public ArrayMemberExpression(VariableExpression array, AbstractEvaluableExpression index, Location location) : base(location)
    {
        Array = array;
        Index = index;
    }
    
    public VariableExpression Array { get; }
    
    public AbstractEvaluableExpression Index { get; }

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

        return arrayValue.Value[index].Evaluate(programContext);
    }
}