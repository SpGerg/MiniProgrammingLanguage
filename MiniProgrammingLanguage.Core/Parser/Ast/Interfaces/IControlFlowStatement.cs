using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

public interface IControlFlowStatement : IStatement
{
    StateType State { get; }
}