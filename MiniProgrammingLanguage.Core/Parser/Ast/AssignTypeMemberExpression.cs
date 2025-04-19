using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignTypeMemberExpression : AbstractEvaluableExpression, IAssignExpression
{
    public AssignTypeMemberExpression(BinaryExpression left, AbstractEvaluableExpression right, Location location) : base(location)
    {
        Left = left;
        Right = right;
    }
    
    public BinaryExpression Left { get; }
    
    public AbstractEvaluableExpression Right { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var left = Left.Dot(programContext);
        var right = Right.Evaluate(programContext);

        if (!left.Type.Is(right))
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(left.Type.ValueType.ToString(), right.Type.ToString(), Left.Location);
        }

        left.Value = right;

        return right;
    }
}