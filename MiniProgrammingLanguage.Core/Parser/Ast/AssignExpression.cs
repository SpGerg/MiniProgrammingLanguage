using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignExpression : AbstractEvaluableExpression, IAssignExpression
{
    public AssignExpression(string name, ObjectTypeValue type, AbstractEvaluableExpression evaluableExpression, FunctionBodyExpression root, Location location, AccessType access) : base(location)
    {
        Name = name;
        Type = type;
        Root = root;
        EvaluableExpression = evaluableExpression;
        Access = access;
    }

    public string Name { get; }
    
    public ObjectTypeValue Type { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public AbstractEvaluableExpression EvaluableExpression { get; }
    
    public AccessType Access { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var value = EvaluableExpression.Evaluate(programContext);

        if (Type is not null && !Type.Is(value))
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(Type.ValueType.ToString(), value.Type.ToString(), Location);
        }

        var instance = new UserVariableInstance
        {
            Name = Name,
            Module = programContext.Module,
            Type = Type ?? ObjectTypeValue.Any,
            Root = Root,
            Access = Access,
            Value = value
        };
        
        if (Type is not null)
        {
            programContext.Variables.Add(instance, Location);

            return value;
        }
        
        programContext.Variables.AddOrSet(programContext, instance, Location);

        return value;
    }
}