namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

public interface ITypeMemberIdentification
{
    string Identificator { get; }

    bool Is(ITypeMemberIdentification identification);
}