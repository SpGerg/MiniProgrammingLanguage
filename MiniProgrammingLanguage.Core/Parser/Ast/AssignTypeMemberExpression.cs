using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignTypeMemberExpression : AbstractEvaluableExpression, IAssignExpression
{
    public AssignTypeMemberExpression(DotExpression left, AbstractEvaluableExpression right, Location location) : base(location)
    {
        Left = left;
        Right = right;
    }
    
    public DotExpression Left { get; }
    
    public AbstractEvaluableExpression Right { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var (type, member) = Left.Dot(programContext);
        var right = Right.Evaluate(programContext);
        
        if (Left.Right is FunctionCallExpression functionCallExpression)
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException("variable", $"{functionCallExpression.Name}()", Left.Location);
        }
        
        if (!member.Type.Is(right))
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(member.Type.ToString(), right.Type.ToString(), Left.Location);
        }

        if (member is not ITypeVariableMemberValue variableMember)
        {
            InterpreterThrowHelper.ThrowCannotAccessException(member.Instance.Identification.Identifier, Left.Location);

            return null;
        }

        var setterContext = new TypeMemberSetterContext
        {
            ProgramContext = programContext,
            Type = type,
            Member = (ITypeLanguageVariableMember) member.Instance,
            Value = right,
            Location = Location
        };
        
        variableMember.SetValue(setterContext);

        return right;
    }
}