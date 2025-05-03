using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
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
    public DotExpression(AbstractEvaluableExpression left, AbstractEvaluableExpression right,
        FunctionBodyExpression root, Location location) : base(location)
    {
        Left = left;
        Right = right;
        Root = root;
    }

    public AbstractEvaluableExpression Left { get; }

    public AbstractEvaluableExpression Right { get; }

    public FunctionBodyExpression Root { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var left = Left.Evaluate(programContext);

        if (left is TypeValue typeValue)
        {
            var (_, member) = Dot(programContext, typeValue);

            if (member is ITypeVariableMemberValue variableMember)
            {
                var context = new TypeMemberGetterContext
                {
                    ProgramContext = programContext,
                    Type = typeValue,
                    Member = member.Instance,
                    Location = Location
                };

                return variableMember.GetValue(context);
            }

            if (member is ITypeFunctionMemberValue functionMember)
            {
                var context = new TypeFunctionExecuteContext
                {
                    ProgramContext = programContext,
                    Type = typeValue,
                    Arguments = ((FunctionCallExpression)Right).Arguments,
                    Member = functionMember.Instance,
                    Root = Root,
                    Location = Location
                };

                return functionMember.GetValue(context);
            }

            return new NoneValue();
        }

        if (left is not EnumValue enumValue || Right is not VariableExpression variableExpression)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Enum.ToString(), left.Type.ToString(),
                Location);

            return null;
        }

        if (!enumValue.Value.TryGetByName(variableExpression.Name, out _))
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(enumValue.Value.Name, variableExpression.Name,
                Location);
        }

        return new EnumMemberValue(enumValue.Value, variableExpression.Name);
    }

    public (TypeValue Type, ITypeMemberValue Member) Dot(ProgramContext context, TypeValue parent = null)
    {
        return parent is null
            ? EvaluateRootExpression(context)
            : EvaluateNestedExpression(context, parent);
    }

    private (TypeValue Type, ITypeMemberValue Member) EvaluateRootExpression(ProgramContext context)
    {
        var leftValue = Left.Evaluate(context);

        if (leftValue is not TypeValue typeValue)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(
                ValueType.Type.ToString(), leftValue.Type.ToString(),
                Location
            );
            return (null, null);
        }

        return Right is DotExpression dotExpression
            ? dotExpression.Dot(context, typeValue)
            : (typeValue, GetTypeMember(context, typeValue, Right));
    }

    private (TypeValue Type, ITypeMemberValue Member) EvaluateNestedExpression(
        ProgramContext context,
        TypeValue parent)
    {
        if (Right is not DotExpression dotExpression)
        {
            return HandleSingleMemberAccess(context, parent);
        }

        var (_, memberType) = ResolveLeftMember(context, parent, dotExpression);

        return dotExpression.Dot(context, memberType);
    }

    private (TypeValue Type, ITypeMemberValue Member) HandleSingleMemberAccess(
        ProgramContext context,
        TypeValue parent)
    {
        var member = GetTypeMember(context, parent, Right);

        if (member is null)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(
                parent.Name,
                "?",
                Location
            );
            return (null, null);
        }

        return (parent, member);
    }

    private ITypeMemberValue GetTypeMember(
        ProgramContext context,
        TypeValue type,
        AbstractEvaluableExpression expression)
    {
        if (expression is VariableExpression variableExpression)
        {
            var result = type.Get(new KeyTypeMemberIdentification
            {
                Identifier = variableExpression.Name
            });

            if (result is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(type.Name, variableExpression.Name, Location);
            }

            return result;
        }

        if (expression is FunctionCallExpression functionCallExpression)
        {
            var typeValue = context.Types.Get(functionCallExpression.Root, type.Name, context.Module,
                Location);
            var function = type.Get(new FunctionTypeMemberIdentification
            {
                Identifier = functionCallExpression.Name
            });

            if (function is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(type.Name, functionCallExpression.Name, Location);
            }

            if (function.Instance is ITypeLanguageFunctionMember languageFunctionMember)
            {
                return new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Instance = function.Instance,
                    Value = languageFunctionMember.Bind.Invoke(new TypeFunctionExecuteContext
                    {
                        ProgramContext = context,
                        Type = type,
                        Member = languageFunctionMember,
                        Arguments = functionCallExpression.Arguments,
                        Root = Root,
                        Location = Location
                    })
                };
            }

            if (function.Instance is not TypeFunctionMemberInstance functionInstance)
            {
                return null;
            }

            if (functionInstance?.Value is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(typeValue.Name, functionCallExpression.Name,
                    expression.Location);
            }

            if (!functionInstance.Value.IsDeclared)
            {
                InterpreterThrowHelper.ThrowFunctionNotDeclaredException(functionInstance.Value.Name, Location);

                return null;
            }

            if (functionInstance.Value is UserFunctionInstance userFunctionInstance)
                context.Variables.AddOrSet(context, new UserVariableInstance
                {
                    Name = "self",
                    Module = "system",
                    Access = AccessType.ReadOnly,
                    Type = new ObjectTypeValue(typeValue.Name, ValueType.Type),
                    Value = type,
                    Root = userFunctionInstance.Body
                }, Location);

            return new TypeMemberValue
            {
                Type = ObjectTypeValue.Function,
                Instance = function.Instance,
                Value = functionInstance.Value.Evaluate(new FunctionExecuteContext
                {
                    ProgramContext = context,
                    Arguments = functionCallExpression.Arguments,
                    Root = Root,
                    Location = Location
                })
            };
        }

        InterpreterThrowHelper.ThrowMemberNotFoundException(type.Name, "?", expression.Location);

        return null;
    }

    private AbstractValue GetMemberValue(
        ProgramContext context,
        TypeValue parent,
        ITypeMemberValue member)
    {
        return member switch
        {
            TypeLanguageFunctionMemberValue languageFunction => ExecuteFunction(context, parent, languageFunction),
            TypeLanguageVariableMemberValue languageVariable => GetVariableValue(context, parent, languageVariable),
            ITypeVariableMemberValue variable => GetVariableValue(context, parent, variable),
            ITypeFunctionMemberValue function => ExecuteFunction(context, parent, function),
            _ => throw new NotSupportedException($"Unsupported member type: {member.GetType()}")
        };
    }

    private AbstractValue GetVariableValue(
        ProgramContext context,
        TypeValue parent,
        ITypeVariableMemberValue variable)
    {
        var getterContext = CreateMemberGetterContext(context, parent, variable.Instance);
        return variable.GetValue(getterContext);
    }

    private AbstractValue ExecuteFunction(
        ProgramContext context,
        TypeValue parent,
        ITypeFunctionMemberValue function)
    {
        var functionContext = CreateFunctionContext(
            context,
            parent,
            function.Instance,
            ((FunctionCallExpression)Right).Arguments
        );
        return function.GetValue(functionContext);
    }

    private (AbstractValue Value, TypeValue Type) ResolveLeftMember(
        ProgramContext context,
        TypeValue parent,
        DotExpression dotExpression)
    {
        var member = GetTypeMember(context, parent, dotExpression.Left);
        var value = GetMemberValue(context, parent, member);

        if (value is not TypeValue typeValue)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(ValueType.Type.ToString(), value.Type.ToString(),
                Location);
            return (null, null);
        }

        return (value, typeValue);
    }

    private TypeMemberGetterContext CreateMemberGetterContext(
        ProgramContext context,
        TypeValue type,
        ITypeMember member)
    {
        return new TypeMemberGetterContext
        {
            ProgramContext = context,
            Type = type,
            Member = member,
            Location = Location
        };
    }

    private TypeFunctionExecuteContext CreateFunctionContext(
        ProgramContext context,
        TypeValue type,
        ITypeMember member,
        AbstractEvaluableExpression[] arguments)
    {
        return new TypeFunctionExecuteContext
        {
            ProgramContext = context,
            Type = type,
            Member = member,
            Arguments = arguments,
            Root = Root,
            Location = Location
        };
    }
}