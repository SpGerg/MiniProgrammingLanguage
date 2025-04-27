using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

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
        var left = Left.Evaluate(programContext);

        if (left is TypeValue typeValue)
        {
            var (_, member) = Dot(programContext, typeValue);
            
            var context = new TypeMemberGetterContext
            {
                ProgramContext = programContext,
                Type = typeValue,
                Member = member.Instance,
                Location = Location
            };
            
            return member.GetValue(context);
        }
        
        if (left is not EnumValue enumValue || Right is not VariableExpression variableExpression)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Enum.ToString(), left.Type.ToString(), Location);

            return null;
        }

        if (!enumValue.Value.TryGetByName(variableExpression.Name, out _))
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(enumValue.Value.Name, variableExpression.Name, Location);
        }

        return new EnumMemberValue(enumValue.Value.Name, variableExpression.Name);
    }

    public (TypeValue, ITypeMemberValue) Dot(ProgramContext context, TypeValue parent = null)
    {
        if (parent is null)
        {
            var left = Left.Evaluate(context);

            if (left is not TypeValue typeValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), left.Type.ToString(), Location);

                return (null, null);
            }

            if (Right is DotExpression dotExpression)
            {
                return dotExpression.Dot(context, typeValue);
            }

            return (typeValue, GetMemberFromType(context, typeValue, Right));
        }
        else
        {
            if (Right is not DotExpression dotExpression)
            {
                var dotMember = GetMemberFromType(context, parent, Right);

                if (dotMember is null)
                {
                    InterpreterThrowHelper.ThrowMemberNotFoundException(parent.Name, "?", Location);
                }
                
                var getterContext = new TypeMemberGetterContext
                {
                    ProgramContext = context,
                    Type = parent,
                    Member = dotMember.Instance,
                    Location = Location
                };

                var value = dotMember.GetValue(getterContext);
                
                if (value is not TypeValue dotTypeValue)
                {
                    return (parent, dotMember);
                }
                
                return (dotTypeValue, GetMemberFromType(context, dotTypeValue, Right));
            }
            
            var left = GetMemberFromType(context, parent, dotExpression.Left);
            
            var leftContext = new TypeMemberGetterContext
            {
                ProgramContext = context,
                Type = parent,
                Member = left.Instance,
                Location = Location
            };

            if (left.GetValue(leftContext) is not TypeValue typeValue)
            {
                InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), left.Type.ToString(), Location);
                    
                return (null, null);
            }

            return dotExpression.Dot(context, typeValue);
        }
    }

    private ITypeMemberValue GetMemberFromType(ProgramContext programContext, TypeValue typeValue, AbstractExpression expression)
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
            var type = programContext.Types.Get(functionCallExpression.Root, typeValue.Name, programContext.Module, Location);
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
                    Module = "system",
                    Access = AccessType.ReadOnly,
                    Type = new ObjectTypeValue(typeValue.Name, ValueType.Type),
                    Value = typeValue,
                    Root = userFunctionInstance.Body,
                }, Location);
            }

            return new TypeMemberValue
            {
                Type = ObjectTypeValue.Function,
                Instance = function,
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