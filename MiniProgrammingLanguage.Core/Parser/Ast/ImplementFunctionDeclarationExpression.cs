using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class ImplementFunctionDeclarationExpression : AbstractEvaluableExpression, IStatement
{
    public ImplementFunctionDeclarationExpression(string type,
        FunctionDeclarationExpression functionDeclarationExpression, Location location) : base(location)
    {
        Type = type;
        FunctionDeclarationExpression = functionDeclarationExpression;
    }

    public string Type { get; }

    public FunctionDeclarationExpression FunctionDeclarationExpression { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var type = programContext.Types.Get(FunctionDeclarationExpression.Root, Type, programContext.Module, Location);

        if (type is not UserTypeInstance userTypeInstance)
        {
            InterpreterThrowHelper.ThrowTypeNotFoundException(Type, Location);

            return null;
        }

        var functionInstance = FunctionDeclarationExpression.Create(programContext.Module);

        var member = userTypeInstance.Get(new FunctionTypeMemberIdentification
        {
            Identifier = functionInstance.Name
        });

        if (member is not TypeFunctionMemberInstance functionMemberInstance)
        {
            InterpreterThrowHelper.ThrowMemberNotFoundException(Type, functionInstance.Name, Location);

            return null;
        }

        functionMemberInstance.Value = functionInstance;

        return new VoidValue();
    }
}