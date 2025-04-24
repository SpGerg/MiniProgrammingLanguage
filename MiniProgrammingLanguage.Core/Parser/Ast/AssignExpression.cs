using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class AssignExpression : AbstractEvaluableExpression, IAssignExpression
{
    public AssignExpression(string name, ObjectTypeValue type, AbstractEvaluableExpression evaluableExpression, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Type = type;
        Root = root;
        EvaluableExpression = evaluableExpression;
    }

    public string Name { get; }
    
    public ObjectTypeValue Type { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public AbstractEvaluableExpression EvaluableExpression { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var value = EvaluableExpression.Evaluate(programContext);

        if (Type is not null && !Type.Is(value))
        {
            InterpreterThrowHelper.ThrowIncorrectTypeException(Type.ValueType.ToString(), value.Type.ToString(), Location);
        }
        
        if (Type is not null)
        {
            programContext.Variables.Add(new UserVariableInstance
            {
                Name = Name,
                Type = Type,
                Root = Root,
                Value = value
            }, Location);

            return value;
        }
        
        programContext.Variables.Set(programContext, new UserVariableInstance
        {
            Name = Name,
            Root = Root,
            Value = value
        }, Location);

        return value;
    }
}