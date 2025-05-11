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

        if (left is AbstractDataContainerValue typeValue)
        {
            var (_, member) = Dot(programContext, typeValue);

            if (member is ITypeVariableMemberValue variableMember)
            {
                var context = new TypeMemberGetterContext
                {
                    ProgramContext = programContext,
                    Type = typeValue as TypeValue,
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
                    Type = typeValue as TypeValue,
                    Arguments = ((FunctionCallExpression)Right).Arguments,
                    Member = functionMember.Instance,
                    Root = Root,
                    Location = Location
                };

                return functionMember.GetValue(context);
            }

            return NoneValue.Instance;
        }

        if (left is EnumValue enumValue)
        {
            if (Right is not VariableExpression variableExpression)
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

            return new EnumMemberValue(enumValue.Value, variableExpression.Name, enumValue.Value.GetByName(variableExpression.Name));
        }


        return left;
    }

    public (AbstractDataContainerValue dataContainer, ITypeMemberValue Member) Dot(ProgramContext context, AbstractDataContainerValue parent = null)
    {
        return parent is null
            ? EvaluateRootExpression(context)
            : EvaluateNestedExpression(context, parent);
    }

    private (AbstractDataContainerValue dataContainer, ITypeMemberValue Member) EvaluateRootExpression(ProgramContext context)
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

    private (AbstractDataContainerValue dataContainer, ITypeMemberValue Member) EvaluateNestedExpression(
        ProgramContext context,
        AbstractDataContainerValue parent)
    {
        if (Right is not DotExpression dotExpression)
        {
            return HandleSingleMemberAccess(context, parent);
        }

        var (_, memberType) = ResolveLeftMember(context, parent, dotExpression);

        return dotExpression.Dot(context, memberType);
    }

    private (AbstractDataContainerValue dataContainer, ITypeMemberValue Member) HandleSingleMemberAccess(
        ProgramContext context,
        AbstractDataContainerValue parent)
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
        AbstractDataContainerValue dataContainer,
        AbstractExpression expression)
    {
        if (expression is VariableExpression variableExpression)
        {
            var result = dataContainer.Get(new KeyTypeMemberIdentification
            {
                Identifier = variableExpression.Name
            });

            if (result is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(dataContainer.Name, variableExpression.Name, Location);
            }

            return result;
        }

        if (expression is FunctionCallExpression functionCallExpression)
        {
            var typeValue = context.Types.Get(functionCallExpression.Root, dataContainer.Name, context.Module,
                Location);
            var function = dataContainer.Get(new FunctionTypeMemberIdentification
            {
                Identifier = functionCallExpression.Name
            });

            if (function is null)
            {
                InterpreterThrowHelper.ThrowMemberNotFoundException(dataContainer.Name, functionCallExpression.Name, Location);
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
                        Type = dataContainer as TypeValue,
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

            if (functionInstance.Value is null)
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
                    Value = dataContainer,
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
        
        InterpreterThrowHelper.ThrowMemberNotFoundException(dataContainer.Name, "?", expression.Location);

        return null;
    }

    private AbstractValue GetMemberValue(
        ProgramContext context,
        AbstractDataContainerValue dataContainer,
        ITypeMemberValue member)
    {
        return member switch
        {
            TypeLanguageFunctionMemberValue languageFunction => ExecuteFunction(context, dataContainer, languageFunction),
            TypeLanguageVariableMemberValue languageVariable => GetVariableValue(context, dataContainer, languageVariable),
            ITypeVariableMemberValue variable => GetVariableValue(context, dataContainer, variable),
            ITypeFunctionMemberValue function => ExecuteFunction(context, dataContainer, function),
            _ => throw new NotSupportedException($"Unsupported member type: {member.GetType()}")
        };
    }

    private AbstractValue GetVariableValue(
        ProgramContext context,
        AbstractDataContainerValue dataContainer,
        ITypeVariableMemberValue variable)
    {
        var getterContext = CreateMemberGetterContext(context, dataContainer, variable.Instance);
        return variable.GetValue(getterContext);
    }

    private AbstractValue ExecuteFunction(
        ProgramContext context,
        AbstractDataContainerValue dataContainer,
        ITypeFunctionMemberValue function)
    {
        var functionContext = CreateFunctionContext(
            context,
            dataContainer,
            function.Instance,
            ((FunctionCallExpression)Right).Arguments
        );
        return function.GetValue(functionContext);
    }

    private (AbstractValue Value, TypeValue Type) ResolveLeftMember(
        ProgramContext context,
        AbstractDataContainerValue dataContainer,
        DotExpression dotExpression)
    {
        var member = GetTypeMember(context, dataContainer, dotExpression.Left);
        var value = GetMemberValue(context, dataContainer, member);

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
        AbstractDataContainerValue dataContainer,
        ITypeMember member)
    {
        return new TypeMemberGetterContext
        {
            ProgramContext = context,
            Type = dataContainer as TypeValue,
            Member = member,
            Location = Location
        };
    }

    private TypeFunctionExecuteContext CreateFunctionContext(
        ProgramContext context,
        AbstractDataContainerValue dataContainer,
        ITypeMember member,
        AbstractEvaluableExpression[] arguments)
    {
        return new TypeFunctionExecuteContext
        {
            ProgramContext = context,
            Type = dataContainer as TypeValue,
            Member = member,
            Arguments = arguments,
            Root = Root,
            Location = Location
        };
    }
}