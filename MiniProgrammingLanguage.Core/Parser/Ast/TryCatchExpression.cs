using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class TryCatchExpression : AbstractEvaluableExpression, IStatement
{
    public TryCatchExpression(FunctionBodyExpression tryBody, FunctionBodyExpression catchBody, Location location) : base(location)
    {
        TryBody = tryBody;
        CatchBody = catchBody;
    }
    
    public FunctionBodyExpression TryBody { get; }
    
    public FunctionBodyExpression CatchBody { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        AbstractValue result;

        try
        {
            result = TryBody.Evaluate(programContext);
        }
        catch (AbstractInterpreterException languageException)
        {
            var exceptionTypeInstance = programContext.Types.Get(null, "__exception", "std", Location);
            var exception = exceptionTypeInstance.Create();

            var nameMember = (TypeMemberValue) exception.Get("name");
            nameMember.Value = new StringValue(languageException.GetType().Name);
            
            var messageMember = (TypeMemberValue) exception.Get("message");
            messageMember.Value = new StringValue(languageException.Message);
            
            var variableInstance = new UserVariableInstance
            {
                Name = "exception",
                Module = "system",
                Root = CatchBody,
                Access = AccessType.ReadOnly,
                Type = new ObjectTypeValue("__exception", ValueType.Type),
                Value = exception
            };
            
            programContext.Variables.Add(variableInstance, programContext.Location);

            CatchBody.Evaluate(programContext);
            
            return VoidValue.Instance;
        }

        return result;
    }
}