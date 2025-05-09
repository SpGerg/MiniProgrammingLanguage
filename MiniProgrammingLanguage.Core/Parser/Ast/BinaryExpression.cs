using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class BinaryExpression : AbstractEvaluableExpression
{
    public BinaryExpression(BinaryOperatorType operatorType, AbstractEvaluableExpression left,
        AbstractEvaluableExpression right, Location location) : base(location)
    {
        Operator = operatorType;
        Left = left;
        Right = right;
    }

    public BinaryOperatorType Operator { get; }

    public AbstractEvaluableExpression Left { get; }

    public AbstractEvaluableExpression Right { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return Operator switch
        {
            _ => EvaluateWithSameValue(programContext)
        };
    }

    private AbstractValue EvaluateWithSameValue(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        var leftType = new ObjectTypeValue(left.Name, left.Type);

        if (!leftType.Is(right) && right is not NoneValue)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }

        var result = Operator switch
        {
            BinaryOperatorType.Plus => Plus(context, left, right),
            BinaryOperatorType.Minus => Minus(context, left, right),
            BinaryOperatorType.Multiplication => Multiplication(context, left, right),
            BinaryOperatorType.Division => Division(context, left, right),
            BinaryOperatorType.And => And(context, left, right),
            BinaryOperatorType.Or => Or(context, left, right),
            BinaryOperatorType.Equals => Equals(context, left, right, false),
            BinaryOperatorType.Greater => Greater(context, left, right),
            BinaryOperatorType.Less => Less(context, left, right),
            BinaryOperatorType.Not => Equals(context, left, right, true),
            _ => null
        };

        if (result is null)
        {
            InterpreterThrowHelper.ThrowCannotAccessException("?", Location);
        }

        return result;
    }

    private AbstractValue And(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return new BooleanValue(left.AsBoolean(context, Location) && right.AsBoolean(context, Location));
    }

    private AbstractValue Or(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return new BooleanValue(left.AsBoolean(context, Location) || right.AsBoolean(context, Location));
    }

    private AbstractValue Greater(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return left.Type switch
        {
            ValueType.Number => new BooleanValue(left.AsNumber(context, Location) > right.AsNumber(context, Location)),
            ValueType.RoundNumber => new BooleanValue(left.AsRoundNumber(context, Location) >
                                                      right.AsRoundNumber(context, Location)),
            _ => null
        };
    }

    private AbstractValue Less(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return left.Type switch
        {
            ValueType.Number => new BooleanValue(left.AsNumber(context, Location) < right.AsNumber(context, Location)),
            ValueType.RoundNumber => new BooleanValue(left.AsRoundNumber(context, Location) <
                                                      right.AsRoundNumber(context, Location)),
            _ => null
        };
    }

    private static AbstractValue Equals(ProgramContext context, AbstractValue left, AbstractValue right, bool isNot)
    {
        var result = left.Is(right);
        
        return new BooleanValue(isNot ? !result : result);
    }

    private AbstractValue Plus(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return left.Type switch
        {
            ValueType.String => new StringValue(left.AsString(context, Location) + right.AsString(context, Location)),
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) + right.AsNumber(context, Location)),
            ValueType.RoundNumber => new NumberValue(left.AsNumber(context, Location) +
                                                     right.AsNumber(context, Location)),
            _ => null
        };
    }

    private AbstractValue Minus(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return left.Type switch
        {
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) - right.AsNumber(context, Location)),
            ValueType.RoundNumber => new NumberValue(left.AsNumber(context, Location) -
                                                     right.AsNumber(context, Location)),
            _ => null
        };
    }

    private AbstractValue Multiplication(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        return left.Type switch
        {
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) * right.AsNumber(context, Location)),
            ValueType.RoundNumber => new NumberValue(left.AsNumber(context, Location) *
                                                     right.AsNumber(context, Location)),
            _ => null
        };
    }

    private AbstractValue Division(ProgramContext context, AbstractValue left, AbstractValue right)
    {
        var rightNumber = right.AsNumber(context, Location);

        if (rightNumber == 0)
        {
            InterpreterThrowHelper.ThrowDivideByZeroException(Location);
        }

        return left.Type switch
        {
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) / rightNumber),
            ValueType.RoundNumber => new NumberValue(left.AsNumber(context, Location) / (int)rightNumber),
            _ => null
        };
    }
}