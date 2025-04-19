using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;

namespace MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

public interface IEvaluableExpression
{
    AbstractValue Evaluate(ProgramContext programContext);
}