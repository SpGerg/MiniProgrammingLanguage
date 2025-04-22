namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeMemberIdentification
{
    string Identifier { get; }

    bool Is(ITypeMemberIdentification identification);
}