using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class BinaryExpression : AbstractEvaluableExpression
{
    public BinaryExpression(BinaryOperatorType operatorType, AbstractEvaluableExpression left, AbstractEvaluableExpression right, Location location) : base(location)
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
            BinaryOperatorType.Plus => Plus(programContext),
            BinaryOperatorType.Minus => Minus(programContext),
            BinaryOperatorType.Equals => Equals(programContext),
            BinaryOperatorType.Greater => Greater(programContext),
            BinaryOperatorType.Less => Less(programContext),
            BinaryOperatorType.Dot => Dot(programContext).Value,
            BinaryOperatorType.And => And(programContext),
            BinaryOperatorType.Or => Or(programContext),
            _ => null
        };
    }

    public TypeMemberValue Dot(ProgramContext context, TypeValue parent = null)
    {
        if (parent is null)
        {
            var left = Left.Evaluate(context);

            if (left is not TypeValue typeValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), left.Type.ToString(), Location);

                return null;
            }

            if (Right is BinaryExpression binaryExpression)
            {
                return binaryExpression.Dot(context, typeValue);
            }

            return GetMemberFromType(typeValue, Right);
        }
        else
        {
            if (Right is not BinaryExpression binaryExpression)
            {
                return GetMemberFromType(parent, Right);
            }
            
            var left = GetMemberFromType(parent, binaryExpression.Left).Value;

            if (left is not TypeValue typeValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), left.Type.ToString(), Location);
                    
                return null;
            }

            return binaryExpression.Dot(context, typeValue);
        }
    }

    private TypeMemberValue GetMemberFromType(TypeValue typeValue, AbstractExpression expression)
    {
        if (expression is VariableExpression variableExpression)
        {
            return typeValue.Get(new KeyTypeMemberIdentification
            {
                Identificator = variableExpression.Name
            });
        }

        return null;
    }

    private AbstractValue And(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }

        return new BooleanValue(left.AsBoolean(context, Location) && right.AsBoolean(context, Location));
    }
    
    private AbstractValue Or(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }

        return new BooleanValue(left.AsBoolean(context, Location) || right.AsBoolean(context, Location));
    }
    
    private AbstractValue Greater(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }
        
        return left.Type switch
        {
            ValueType.Number => new BooleanValue(left.AsNumber(context, Location) > right.AsNumber(context, Location)),
            ValueType.RoundNumber => new BooleanValue(left.AsRoundNumber(context, Location) > right.AsRoundNumber(context, Location)),
            _ => null
        };
    }
    
    private AbstractValue Less(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }
        
        return left.Type switch
        {
            ValueType.Number => new BooleanValue(left.AsNumber(context, Location) < right.AsNumber(context, Location)),
            ValueType.RoundNumber => new BooleanValue(left.AsRoundNumber(context, Location) < right.AsRoundNumber(context, Location)),
            _ => null
        };
    }

    private AbstractValue Equals(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }
        
        return left.Type switch
        {
            ValueType.String => new BooleanValue(left.AsString(context, Location) == right.AsString(context, Location)),
            ValueType.Number => new BooleanValue(left.AsNumber(context, Location) == right.AsNumber(context, Location)),
            ValueType.Boolean => new BooleanValue(left.AsBoolean(context, Location) == right.AsBoolean(context, Location)),
            ValueType.RoundNumber => new BooleanValue(left.AsRoundNumber(context, Location) == right.AsRoundNumber(context, Location)),
            _ => null
        };
    }
    
    private AbstractValue Plus(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }

        return left.Type switch
        {
            ValueType.String => new StringValue(left.AsString(context, Location) + right.AsString(context, Location)),
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) + right.AsNumber(context, Location)),
            ValueType.RoundNumber => new RoundNumberValue(left.AsRoundNumber(context, Location) + right.AsRoundNumber(context, Location)),
            _ => null
        };
    }
    
    private AbstractValue Minus(ProgramContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        if (left.Type != right.Type)
        {
            InterpreterThrowHelper.ThrowCannotCastException(left.Type.ToString(), right.Type.ToString(), Location);
        }

        return left.Type switch
        {
            ValueType.Number => new NumberValue(left.AsNumber(context, Location) - right.AsNumber(context, Location)),
            _ => null
        };
    }
}