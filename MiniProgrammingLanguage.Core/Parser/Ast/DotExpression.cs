using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class DotExpression : AbstractEvaluableExpression, IStatement
{
    public DotExpression(AbstractEvaluableExpression left, AbstractEvaluableExpression right, Location location) : base(location)
    {
        Left = left;
        Right = right;
    }

    public AbstractEvaluableExpression Left { get; }
    
    public AbstractEvaluableExpression Right { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        return Dot(programContext).Value;
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

            if (Right is DotExpression dotExpression)
            {
                return dotExpression.Dot(context, typeValue);
            }

            return GetMemberFromType(context, typeValue, Right);
        }
        else
        {
            if (Right is not DotExpression dotExpression)
            {
                var dotMember = GetMemberFromType(context, parent, Left).Value;
                
                if (dotMember is not TypeValue dotTypeValue)
                {
                    InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), dotMember.Type.ToString(), Location);

                    return null;
                }
                
                return GetMemberFromType(context, dotTypeValue, Right);
            }
            
            var left = GetMemberFromType(context, parent, dotExpression.Left).Value;

            if (left is not TypeValue typeValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), left.Type.ToString(), Location);
                    
                return null;
            }

            return dotExpression.Dot(context, typeValue);
        }
    }

    private TypeMemberValue GetMemberFromType(ProgramContext programContext, TypeValue typeValue, AbstractExpression expression)
    {
        if (expression is VariableExpression variableExpression)
        {
            var result = typeValue.Get(new KeyTypeMemberIdentification
            {
                Identifier = variableExpression.Name
            });

            return result;
        }
        
        if (expression is FunctionCallExpression functionCallExpression)
        {
            var type = (UserTypeInstance) programContext.Types.Get(functionCallExpression.Root, typeValue.Name, Location);
            var function = type.Get(new FunctionTypeMemberIdentification
            {
                Identifier = functionCallExpression.Name
            }) as TypeFunctionMemberInstance;

            if (function?.Value is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(typeValue.Name, functionCallExpression.Name, expression.Location);
            }

            if (!function.Value.IsDeclared)
            {
                InterpreterThrowHelper.ThrowFunctionNotDeclaredException(function.Value.Name, Location);

                return null;
            }

            if (function.Value is UserFunctionInstance userFunctionInstance)
            {
                programContext.Variables.AddOrSet(programContext, new UserVariableInstance
                {
                    Name = "self",
                    ObjectType = new ObjectTypeValue(typeValue.Name, ValueType.Type),
                    Value = typeValue,
                    Root = userFunctionInstance.Body
                }, Location);
            }

            return new TypeMemberValue
            {
                Type = ObjectTypeValue.Function,
                Value = function.Value.Evaluate(new FunctionExecuteContext
                {
                    ProgramContext = programContext,
                    Arguments = functionCallExpression.Arguments,
                    Location = Location
                })
            };
        }

        InterpreterThrowHelper.ThrowMemberNotFoundException(typeValue.Name, "?", expression.Location);

        return null;
    }
}